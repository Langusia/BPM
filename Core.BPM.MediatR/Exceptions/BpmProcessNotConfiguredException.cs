﻿namespace Core.BPM.MediatR.Exceptions;

public class BpmProcessNotConfiguredException : Exception
{
    private BpmProcessNotConfiguredException(string processName) : base(
        $"Process with name '{processName}' is not configured")
    {
    }

    public static BpmProcessNotConfiguredException For<TP>() =>
        new BpmProcessNotConfiguredException(typeof(TP).Name);
}