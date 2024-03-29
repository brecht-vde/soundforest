﻿using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace SoundForest.UnitTests.Common;
public sealed class AutoMoqDataAttribute : AutoDataAttribute
{
    public static IFixture AutoMoqFixture
        => new Fixture().Customize(new AutoMoqCustomization());

    public AutoMoqDataAttribute()
      : base(() => AutoMoqFixture)
    {
    }
}
