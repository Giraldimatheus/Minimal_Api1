using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minimal_Api1.Data;
using Minimal_Api1.Models;
using Minimal_Api1.ViewModel;
using MiniValidation;
using NetDevPack.Identity.Jwt;
using NetDevPack.Identity.Model;

namespace Minimal_Api1.Endpoints
{
    public class Endpoints
    {
        public static void MapActions(WebApplication app)
        {
            // --------------------------------
            app.MapPost("/AuthUser", [AllowAnonymous] async (
                SignInManager<IdentityUser> signInManager,
                UserManager<IdentityUser> userManager,
                IOptions<AppJwtSettings> options,
                RegisterUser registerUser) =>
            {
                if (registerUser == null)
                    return Results.BadRequest("Usuário não informado.");
                if (!MiniValidator.TryValidate(registerUser, out var errors))
                    return Results.ValidationProblem(errors);

                var user = new IdentityUser
                {
                    UserName = registerUser.Email,
                    Email = registerUser.Email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, registerUser.Password);

                if (!result.Succeeded)
                    return Results.BadRequest(result.Errors);

                var jwt = new JwtBuilder()
                            .WithUserManager(userManager)
                            .WithJwtSettings(options.Value)
                            .WithEmail(user.Email)
                            .WithJwtClaims()
                            .WithUserRoles()
                            .BuildUserResponse();

                return Results.Ok(jwt);

            }).ProducesValidationProblem()
                  .Produces(StatusCodes.Status200OK)
                  .Produces(StatusCodes.Status400BadRequest)
                  .WithName("RegisterUser")
                  .WithTags("User");

            // --------------------------------
            app.MapPost("/login", [AllowAnonymous] async (
                SignInManager<IdentityUser> signInManager,
                UserManager<IdentityUser> userManager,
                IOptions<AppJwtSettings> options,
                LoginUser loginUser) =>
            {
                if (loginUser == null)
                    return Results.BadRequest("Usuário não informado.");
                if (!MiniValidator.TryValidate(loginUser, out var errors))
                    return Results.ValidationProblem(errors);

                var result = await signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, false);

                if (!result.Succeeded)
                    return Results.BadRequest("Usuário ou senha inválidos");

                var jwt = new JwtBuilder()
                                .WithUserManager(userManager)
                                .WithJwtSettings(options.Value)
                                .WithEmail(loginUser.Email)
                                .WithJwtClaims()
                                .WithUserRoles()
                                .BuildUserResponse();

                return Results.Ok(jwt);
            }).ProducesValidationProblem()
              .Produces(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status400BadRequest)
              .WithName("LoginUser")
              .WithTags("User");

            // --------------------------------
            app.MapGet("/Champions", [AllowAnonymous] async (
                ChampionsContext Context,
                IMapper _mapper) =>
            {
                var lista = await Context.Champions.ToListAsync();
                var listaVM = _mapper.Map<List<ChampionViewModel>>(lista);
                return listaVM;
            })
                .WithName("GetChampions")
                .WithTags("Champion");

            // --------------------------------
            app.MapGet("/Champions/{id}", [AllowAnonymous] async (
                int id,
                ChampionsContext Context) =>

                await Context.Champions.FindAsync(id)
                is Champions champion ? Results.Ok(champion) : Results.NotFound())
                .Produces<Champions>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("GetChampionById")
                .WithTags("Champion");

            // --------------------------------
            app.MapPost("/Champions", [Authorize] async (
                ChampionsContext context,
                ChampionViewModel championVM,
                IMapper _mapper) =>
            {
                if (!MiniValidator.TryValidate(championVM, out var errors))
                    return Results.ValidationProblem(errors);

                var champion = _mapper.Map<Champions>(championVM);
                context.Champions.Add(champion);
                var result = await context.SaveChangesAsync();
                return result > 0 ? Results.CreatedAtRoute("GetChampionById", new { id = champion.Id }, champion)
                                  : Results.BadRequest("Houveram problemas ao salvar o registro no banco.");

            }).ProducesValidationProblem()
              .Produces<Champions>(StatusCodes.Status201Created)
              .Produces(StatusCodes.Status400BadRequest)
              .WithName("PostChampion")
              .WithTags("Champion");

            // --------------------------------
            app.MapPut("/Champion/{id}", [Authorize] async (
                int id,
                ChampionsContext context,
                ChampionViewModel championVM,
                IMapper _mapper) =>
            {
                var championBanco = await context.Champions.AsNoTracking<Champions>()
                                                           .FirstOrDefaultAsync(x => x.Id == id);

                if (championBanco == null) return Results.NotFound();

                if (!MiniValidator.TryValidate(championVM, out var errors))
                    return Results.ValidationProblem(errors);

                var champion = _mapper.Map<Champions>(championVM);
                champion.Id = id;

                context.Champions.Update(champion);
                var result = await context.SaveChangesAsync();

                return result > 0 ? Results.NoContent()
                                  : Results.BadRequest("Houveram problemas ao salvar o registro no banco.");
            }).ProducesValidationProblem()
              .Produces(StatusCodes.Status204NoContent)
              .Produces(StatusCodes.Status400BadRequest)
              .WithName("PutChampion")
              .WithTags("Champion");

            // --------------------------------
            app.MapDelete("/Champion/{id}", [Authorize] async (
                int id,
                ChampionsContext context) =>
            {
                var championBanco = await context.Champions.AsNoTracking<Champions>()
                                                           .FirstOrDefaultAsync(x => x.Id == id);

                if (championBanco == null) return Results.NotFound();
                context.Champions.Remove(championBanco);
                var result = await context.SaveChangesAsync();

                return result > 0 ? Results.NoContent() : Results.BadRequest("Houve um problema ao excluir o registro.");
            }).Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status204NoContent)
              .Produces(StatusCodes.Status404NotFound)
              //.RequireAuthorization("ExcluirChampion")
              .WithName("DeleteChampion")
              .WithTags("Champion");

            // --------------------------------
        }
    }
}
