using DibBase.Extensions;
using DibBase.Infrastructure;
using DibBaseSampleApi.Controllers;
using DsCore.ApiClient;
using DsCore.Api.Models;
using DsStorage.ApiClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User = DsCore.Api.Models.User;

namespace DsCore.Api;

[ApiController]
[Route("[controller]")]
public class UserController(Repository<User> repo, Repository<Follow> followRepo, DsStorageClientFactory dsStorage) : EntityController<User>(repo)
{
    const int ONLINE_TIME_TRESHOLD = 2;

    [Authorize]
    [HttpPost]
    public override async Task<ActionResult<Guid>> Add(User entity, CancellationToken ct) => await base.Add(entity, ct);

    [Authorize]
    [HttpPut]
    public override async Task<ActionResult<Guid>> Update(User entity, CancellationToken ct)
    {
        if (!HttpContext.IsUser(entity.Guid)) return Unauthorized();
        return await base.Update(entity, ct);
    }

    [Authorize]
    [HttpDelete]
    public override async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var item = await repo.GetById(id.Deobfuscate().Id, ct: ct);
        if (item == null) return Problem();
        if (!HttpContext.IsUser(item.Guid)) return Unauthorized();

        return await base.Delete(id, ct);
    }

    [HttpGet]
    [Route("ReportOnline/{id}")]
    public async Task<ActionResult> ReportOnline(Guid id, CancellationToken ct)
    {
        if (HttpContext.User.Claims.First(x => x.Type == "userId").Value != id.ToString())
            return Unauthorized();
        
        var user = await repo.GetById(id.Deobfuscate().Id, ct: ct);
        if (user == null) return Problem();

        user.LastOnline = DateTime.UtcNow;
        await repo.UpdateAsync(user, ct);
        await repo.CommitAsync(ct);
        return Ok();
    }

    [HttpGet]
    [Route("IsOnline/{id}")]
    public async Task<ActionResult<bool>> IsOnline(Guid id, CancellationToken ct) =>
        Ok(DateTime.UtcNow - (await repo.GetById(id.Deobfuscate().Id, ct: ct))?.LastOnline < TimeSpan.FromMinutes(ONLINE_TIME_TRESHOLD));

    [HttpGet]
    [Route("Followers/{id}")]
    public async Task<ActionResult<List<User>>> GetFollowers(Guid id, CancellationToken ct = default)
    {
        var userId = id.Deobfuscate().Id;
        var ids = (await followRepo.GetAll(restrict: x => x.FollowerId == userId, ct: ct)).Select(x => x.FollowedId);
        return Ok(await repo.GetByIds(ids, ct: ct));
    }

    [HttpGet]
    [Route("Following/{id}")]
    public async Task<ActionResult<List<User>>> GetFollowing(Guid id, CancellationToken ct = default)
    {
        var userId = id.Deobfuscate().Id;
        var ids = (await followRepo.GetAll(restrict: x => x.FollowedId == userId, ct: ct)).Select(x => x.FollowerId);
        return Ok(await repo.GetByIds(ids, ct: ct));
    }

    [HttpGet]
    [Route("GetId/{alias}")]
    public async Task<ActionResult<Guid?>> GetId(string alias, CancellationToken ct) =>
        Ok((await repo.GetAll(restrict: x => x.Alias == alias, ct: ct)).FirstOrDefault()?.Guid);

    [HttpPost]
    [Authorize]
    [Route("UploadProfileImage")]
    public async Task<ActionResult<string>> UploadProfileImage(IFormFile file, CancellationToken ct)
    {
        var userGuid = HttpContext.GetUserGuid();
        var token = HttpContext.GetBearerToken();

        if (userGuid == null || token == null)
            return Unauthorized();

        var userId = ((Guid)userGuid).Deobfuscate().Id;
        var user = await repo.GetById(userId, ct: ct);
        if (user == null) return Unauthorized();
        
        var client = dsStorage.CreateClient(token);
        var filename = await client.Storage_UploadFileAsync(new DsStorage.ApiClient.FileParameter(file.OpenReadStream(), file.FileName, file.ContentType), ct);
        
        user.ProfileImage = filename;
        await repo.UpdateAsync(user, ct);
        await repo.CommitAsync(ct);

        return Ok(filename);
    }
}