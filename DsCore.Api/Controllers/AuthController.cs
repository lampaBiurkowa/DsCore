using DibBase.Infrastructure;
using DsCore.Api.Helpers;
using DsCore.Api.Models;
using DsCore.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using User = DsCore.Api.Models.User;
using DibBase.Extensions;
using DsCryptoLib;
using DsCore.Events;

namespace DsCore.Api;

[ApiController]
[Route("[controller]")]
public class AuthController(Repository<Credentials> repo, Repository<User> userRepo, IOptions<TokenOptions> options) : ControllerBase
{
    readonly Repository<User> userRepo = userRepo;
    readonly TimeSpan verificationCodeValidityTime = TimeSpan.FromMinutes(2);

    [HttpPost("login/{userGuid}")]
    public async Task<ActionResult<string>> Login(Guid userGuid, string passwordBase64, CancellationToken ct)
    {
        var credentials = (await repo.GetAll(restrict: x => x.UserId == userGuid.Deobfuscate().Id, ct: ct)).FirstOrDefault();
        if (credentials == null)
            return Unauthorized();

        var passwordNewHash = SecretsBuilder.CreatePasswordHash(passwordBase64, credentials.Salt);
        if (credentials.Password != passwordNewHash)
            return Unauthorized();
        
        var token = JwtBuilder.BuildToken(userGuid, options.Value);
        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<ActionResult<Guid>> Register(User user, string passwordBase64, CancellationToken ct)
    {
        await userRepo.InsertAsync(user, ct);
        var salt = SecretsBuilder.GenerateSalt();
        var passwordNewHash = SecretsBuilder.CreatePasswordHash(passwordBase64, salt);
        var verificationCode = SecretsBuilder.GenerateVerificationCode();
        var credentials = new Credentials()
        {
            Password = passwordNewHash,
            Salt = salt,
            User = user,
            VerificationCode = SecretsBuilder.TextToBase64(verificationCode),
            VerificationCodeValidUntil = DateTime.UtcNow + verificationCodeValidityTime
        };
        
        await repo.InsertAsync(credentials, ct);
        await repo.CommitAsync(ct);
        await repo.RegisterEvent(new VerificationCodeEvent { UserGuid = user.Guid, VerificationCode = verificationCode }, ct);
        await repo.CommitAsync(ct);

        return Ok(user.Guid);
    }

    [HttpPost("activate/{userGuid}")]
    public async Task<IActionResult> Activate(Guid userGuid, string verificationCodeBase64, CancellationToken ct)
    {
        var credentials = (await repo.GetAll(restrict: x => x.UserId == userGuid.Deobfuscate().Id, ct: ct)).FirstOrDefault();
        if (credentials == null)
            return Unauthorized();
        
        if (credentials.VerificationCode != verificationCodeBase64 || credentials.VerificationCodeValidUntil < DateTime.UtcNow)
            return Unauthorized();

        credentials.IsActivated = true;
        await repo.UpdateAsync(credentials, ct);
        await repo.RegisterEvent(new RegisteredEvent { UserGuid = userGuid }, ct);
        await repo.CommitAsync(ct);

        return Ok();
    }

    [HttpPost("activate/{userGuid}/regenerate-code")]
    public async Task<IActionResult> RegenerateCode(Guid userGuid, CancellationToken ct)
    {
        var credentials = (await repo.GetAll(restrict: x => x.UserId == userGuid.Deobfuscate().Id, ct: ct)).FirstOrDefault();
        if (credentials == null)
            return Unauthorized();
        
        if (credentials.VerificationCodeValidUntil > DateTime.UtcNow)
            return BadRequest();

        var verificationCode = SecretsBuilder.GenerateVerificationCode();
        credentials.VerificationCode = SecretsBuilder.TextToBase64(verificationCode);
        credentials.VerificationCodeValidUntil = DateTime.UtcNow + verificationCodeValidityTime;
        await repo.UpdateAsync(credentials, ct);
        await repo.RegisterEvent(new VerificationCodeEvent { UserGuid = userGuid, VerificationCode = verificationCode }, ct);
        await repo.CommitAsync(ct);

        return Ok();
    }

    [HttpGet("is-activated/{userGuid}")]
    public async Task<ActionResult<bool>> IsActivated(Guid userGuid, CancellationToken ct)
    {
        var credentials = (await repo.GetAll(restrict: x => x.UserId == userGuid.Deobfuscate().Id, ct: ct)).FirstOrDefault();
        if (credentials == null)
            return Ok(false);
        
        return Ok(credentials.IsActivated);
    }

    [HttpPost("changePassword/{userGuid}")]
    public async Task<ActionResult> ChangePassword(Guid userGuid, string oldPasswordBase64, string newPasswordBase64, CancellationToken ct)
    {
        var credentials = (await repo.GetAll(restrict: x => x.UserId == userGuid.Deobfuscate().Id, ct: ct)).FirstOrDefault();
        if (credentials == null)
            return Unauthorized();

        var oldPasswordHash = SecretsBuilder.CreatePasswordHash(oldPasswordBase64, credentials.Salt);
        if (credentials.Password != oldPasswordHash)
            return Unauthorized();
        
        var newPasswordHash = SecretsBuilder.CreatePasswordHash(newPasswordBase64, credentials.Salt);
        credentials.Password = newPasswordHash;
        await repo.UpdateAsync(credentials, ct);
        await repo.CommitAsync(ct);

        return Ok();
    }
}