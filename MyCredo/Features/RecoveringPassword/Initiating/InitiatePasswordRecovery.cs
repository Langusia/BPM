﻿using Core.BPM;
using Core.BPM.MediatR;
using Core.BPM.MediatR.Mediator;
using Credo.Core.Shared.Library;
using Credo.Core.Shared.Mediator;
using MediatR;
using MyCredo.Common;

namespace MyCredo.Features.RecoveringPassword.Initiating;

[BpmRequest<PasswordRecovery>]
public record InitiatePasswordRecovery(
    string PersonalNumber,
    DateTime BirthDate,
    ChannelTypeEnum ChannelType)
    : IBpmRootCommand<Guid>;

public class InitiatePasswordRecoveryHandler(BpmProcessManager<PasswordRecovery> mgr)
    : ICommandHandler<InitiatePasswordRecovery, Guid>
{
    public async Task<Result<Guid>> Handle(InitiatePasswordRecovery request, CancellationToken cancellationToken)
    {
        //dostuff
        var agg = PasswordRecovery.Initiate(request.PersonalNumber, request.BirthDate, request.ChannelType);
        await mgr.StartProcess(
            agg,
            cancellationToken);

        return Result.Success(agg.Id);
    }
}