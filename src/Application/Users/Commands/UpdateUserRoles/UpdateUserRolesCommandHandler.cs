using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Users.Commands.UpdateUserRoles;

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _currentUser;

    public UpdateUserRolesCommandHandler(IIdentityService identityService, IUser currentUser)
    {
        _identityService = identityService;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        // An admin locking themselves out of the admin panel is the kind of mistake that's
        // hard to recover from without direct DB access — block it outright rather than trust
        // the UI alone to prevent it.
        if (request.UserId == _currentUser.Id && !request.Roles.Contains(Roles.Administrator))
            throw new ConflictException("You cannot remove your own Administrator role.");

        var currentRoles = await _identityService.GetRolesAsync(request.UserId);

        var rolesToAdd = request.Roles.Except(currentRoles);
        var rolesToRemove = currentRoles.Except(request.Roles);

        foreach (var role in rolesToAdd)
        {
            var result = await _identityService.AddToRoleAsync(request.UserId, role);
            if (!result.Succeeded)
                throw new ConflictException(string.Join("; ", result.Errors));
        }

        foreach (var role in rolesToRemove)
        {
            var result = await _identityService.RemoveFromRoleAsync(request.UserId, role);
            if (!result.Succeeded)
                throw new ConflictException(string.Join("; ", result.Errors));
        }
    }
}
