using DibBase.Infrastructure;
using DsIdentity.Api.Helpers;
using DsIdentity.Models;
using DsIdentity.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using User = DsIdentity.Models.User;
using DibBase.Extensions;

namespace DsIdentity.Api;

[ApiController]
[Route("[controller]")]
public class AuthController(Repository<Credentials> repo, Repository<User> userRepo, IOptions<TokenOptions> options) : ControllerBase
{
    readonly Repository<User> userRepo = userRepo;

    [HttpPost("login/{userGuid}")]
    public async Task<ActionResult<string>> Login(Guid userGuid, string passwordBase64, CancellationToken ct)
    {
        var credentials = (await repo.GetAll(restrict: x => x.UserId == userGuid.Deobfuscate().Id, ct: ct)).FirstOrDefault();
        if (credentials == null)
            return Unauthorized();

        var passwordNewHash = SecretsBuilder.CreatePasswordHash(passwordBase64, credentials.Salt);
        if (credentials.Password != passwordNewHash)
            return Unauthorized();
        
        var token = SecretsBuilder.BuildToken(userGuid, options.Value);
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
            VerificationCode = SecretsBuilder.TextToBase64(verificationCode)
        };
        
        await repo.InsertAsync(credentials, ct);
        await repo.CommitAsync(ct);

        return Ok(user.Guid);
    }

    [HttpPost("activate/{userGuid}")]
    public async Task<IActionResult> Activate(Guid userGuid, string verificationCodeBase64, CancellationToken ct)
    {
        var credentials = (await repo.GetAll(restrict: x => x.UserId == userGuid.Deobfuscate().Id, ct: ct)).FirstOrDefault();
        if (credentials == null)
            return Unauthorized();
        
        if (credentials.VerificationCode != verificationCodeBase64)
            return Unauthorized();

        credentials.IsActivated = true;
        await repo.UpdateAsync(credentials, ct);
        await repo.CommitAsync(ct);

        return Ok();
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