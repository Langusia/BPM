﻿using Core.BPM;
using Core.BPM.BCommand;
using Core.BPM.Extensions;
using Core.BPM.Interfaces.Builder;
using Core.BPM.MediatR;
using Marten.Events;
using MyCredo.Common;
using MyCredo.Features.RecoveringPassword.ChallengingSecurityQuestion;
using MyCredo.Features.RecoveringPassword.CheckingCard;
using MyCredo.Features.RecoveringPassword.Finishing;
using MyCredo.Features.RecoveringPassword.IdentifyingFace;
using MyCredo.Features.RecoveringPassword.Initiating;
using MyCredo.Features.RecoveringPassword.RequestingPhoneChange;
using MyCredo.Features.TwoFactor;

namespace MyCredo.Features.RecoveringPassword;

public class PasswordRecovery : Aggregate
{
    public PasswordRecovery()
    {
    }

    public PasswordRecovery(string personalNumber, DateTime birthDate, ChannelTypeEnum channelType)
    {
        Id = Guid.NewGuid();
        var @event = new PasswordRecoveryInitiated(personalNumber, birthDate, channelType);
        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(PasswordRecoveryInitiated @event)
    {
        PersonalNumber = @event.PersonalNumber;
        BirthDate = @event.BirthDate;
        ChannelType = @event.ChannelType;
    }

    public static PasswordRecovery Initiate(string personalNumber, DateTime birthDate, ChannelTypeEnum channelType) =>
        new(personalNumber, birthDate, channelType);

    public void ValidateSecurityQuestion()
    {
        var @event = new SecurityQuestionValidated();
        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(IEvent<BpmEvent> @event)
    {
        if (!Counters.ContainsKey(@event.EventType.Name))
            Counters.Add(@event.EventType.Name, 1);
        else
            Counters[@event.EventType.Name] += 1;
    }

    public void Apply(GeneratedOtp @event)
    {
        var a = 1;
    }

    public void Apply(SecurityQuestionValidated @event)
    {
    }

    public void CheckCardComplete()
    {
        var @event = new CheckCardCompleted();
        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(CheckCardCompleted @event)
    {
    }

    public void CheckCardInitiate()
    {
        var @event = new CheckCardInitiated();
        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(CheckCardInitiated @event)
    {
    }

    public void FinishPasswordRecovery()
    {
        var @event = new FinishedPasswordRecovery();
        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(FinishedPasswordRecovery @event)
    {
    }

    public void IdentifyFace()
    {
        var @event = new IdentifiedFace();
        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(IdentifiedFace @event)
    {
    }


    public void PhoneChangeComplete()
    {
        var @event = new PhoneChangeCompleted();
        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(PhoneChangeCompleted @event)
    {
    }

    public void PhoneChangeInitiate()
    {
        var @event = new PhoneChangeInitiated();
        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(PhoneChangeInitiated @event)
    {
    }

    public void Apply(OtpSubmited @event)
    {
        IsOtpValid = @event.IsValid;
    }


    public string PersonalNumber { get; set; }
    public DateTime BirthDate { get; set; }
    public ChannelTypeEnum ChannelType { get; set; }
    public bool IsOtpValid { get; set; }
    public int? KycParameters { get; set; }
}

public class PasswordRecoveryDefinition : BpmDefinition<PasswordRecovery>
{
    public override void DefineProcess(IProcessBuilder<PasswordRecovery> configure)
    {
        configure
            .StartWith<InitiatePasswordRecovery>()
            .Continue<GenerateOtp>(g => g
                .ThenContinue<ValidateOtp>(v => v
                    .ThenContinue<CheckCardInitiate>(ci => ci
                        .ThenContinue<CheckCardComplete>())
                    .Or<ValidateSecurityQuestion>()
                    .Or<IdentifyFace>())
                .Or<PhoneChangeInitiate>(vpc => vpc
                    .ThenContinue<PhoneChangeComplete>(z => z
                        .ThenContinue<CheckCardInitiate>(zz => zz
                            .ThenContinue<CheckCardComplete>())
                        .Or<ValidateSecurityQuestion>())))
            .Continue<FinishPasswordRecovery>();
    }

    public override void SetEventConfiguration(BpmEventConfigurationBuilder<PasswordRecovery> bpmEventConfiguration)
    {
        bpmEventConfiguration.AddBpmEventOptions<GeneratedOtp>(x => x.PermittedTryCount = 3);
    }
}