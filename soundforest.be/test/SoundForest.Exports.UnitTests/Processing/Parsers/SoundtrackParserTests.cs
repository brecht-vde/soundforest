using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.HttpClient;
using SoundForest.Exports.Processing.Domain;
using SoundForest.Exports.Processing.Infrastructure.Parsers;
using SoundForest.UnitTests.Common;
using Xunit;

namespace SoundForest.Exports.UnitTests.Processing.Parsers;
public sealed class SoundtrackParserTests
{
    private static string ValidHtml = """<html><body><ul class="ipc-metadata-list ipc-metadata-list--dividers-between sc-10f549c9-0 hkAQcw ipc-metadata-list--base" role="presentation"><li role="presentation" class="ipc-metadata-list__item ipc-metadata-list__item--stacked" data-testid="list-item"><button class="ipc-metadata-list-item__label" role="button" tabindex="0" aria-disabled="false">If I Didn't Care</button><div class="ipc-metadata-list-item__content-container"><ul class="ipc-inline-list ipc-inline-list--show-dividers ipc-inline-list--inline ipc-metadata-list-item__list-content base" role="presentation"><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div">by <a class="ipc-md-link ipc-md-link--entity" href="/name/nm0492805/?ref_=ttsnd">Jack Lawrence</a></div></div><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div">Performed by <a class="ipc-md-link ipc-md-link--entity" href="/name/nm1311414/?ref_=ttsnd">The Ink Spots</a></div></div><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div">Courtesy of MCA Records</div></div></ul></div></li><li role="presentation" class="ipc-metadata-list__item ipc-metadata-list__item--single-line ipc-metadata-list-item--expandable" data-testid="expandable-list-item"><button class="ipc-metadata-list-item__label" role="button" tabindex="0" aria-disabled="false">Duettino - Sull'aria</button><div class="ipc-metadata-list-item__content-container"><ul class="ipc-inline-list ipc-inline-list--show-dividers ipc-inline-list--no-wrap ipc-inline-list--inline ipc-metadata-list-item__list-content base" role="presentation"><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div">from opera "Le nozze di Figaro (The Marriage of Figaro)"</div></div><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div" style="display:none">Composed by <a class="ipc-md-link ipc-md-link--entity" href="/name/nm0003665/?ref_=ttsnd">Wolfgang Amadeus Mozart</a></div></div><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div" style="display:none">Libretto by <a class="ipc-md-link ipc-md-link--entity" href="/name/nm0196153/?ref_=ttsnd">Lorenzo da Ponte</a> (uncredited)</div></div><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div" style="display:none">Performed by <a class="ipc-md-link ipc-md-link--entity" href="/name/nm0558911/?ref_=ttsnd">Edith Mathis</a> (uncredited) and <a class="ipc-md-link ipc-md-link--entity" href="/name/nm0417916/?ref_=ttsnd">Gundula Janowitz</a> (uncredited)</div></div><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div" style="display:none">Chor und <a class="ipc-md-link ipc-md-link--entity" href="/name/nm2926753/?ref_=ttsnd">Orchester der Deutschen Oper Berlin</a></div></div><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div" style="display:none">Conducted by <a class="ipc-md-link ipc-md-link--entity" href="/name/nm0127037/?ref_=ttsnd">Karl Böhm</a></div></div><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div" style="display:none">Courtesy of Deutsche Grammophon, by arrangement with PolyGram Special Markets</div></div></ul></div><button class="ipc-metadata-list-item__icon-link" role="button" tabindex="0" aria-disabled="false"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" class="ipc-icon ipc-icon--expand-more" id="iconContext-expand-more" viewBox="0 0 24 24" fill="currentColor" role="presentation"><path opacity=".87" fill="none" d="M24 24H0V0h24v24z"></path><path d="M15.88 9.29L12 13.17 8.12 9.29a.996.996 0 1 0-1.41 1.41l4.59 4.59c.39.39 1.02.39 1.41 0l4.59-4.59a.996.996 0 0 0 0-1.41c-.39-.38-1.03-.39-1.42 0z"></path></svg></button></li><li role="presentation" class="ipc-metadata-list__item ipc-metadata-list__item--stacked" data-testid="list-item"><button class="ipc-metadata-list-item__label" role="button" tabindex="0" aria-disabled="false">Put The Blame On Mame</button><div class="ipc-metadata-list-item__content-container"><ul class="ipc-inline-list ipc-inline-list--show-dividers ipc-inline-list--inline ipc-metadata-list-item__list-content base" role="presentation"><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div">by <a class="ipc-md-link ipc-md-link--entity" href="/name/nm0730814/?ref_=ttsnd">Allan Roberts</a> and <a class="ipc-md-link ipc-md-link--entity" href="/name/nm0279463/?ref_=ttsnd">Doris Fisher</a></div></div><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div">Heard on "Gilda" soundtrack</div></div></ul></div></li><li role="presentation" class="ipc-metadata-list__item ipc-metadata-list__item--stacked" data-testid="list-item"><button class="ipc-metadata-list-item__label" role="button" tabindex="0" aria-disabled="false">Lovesick Blues</button><div class="ipc-metadata-list-item__content-container"><ul class="ipc-inline-list ipc-inline-list--show-dividers ipc-inline-list--inline ipc-metadata-list-item__list-content base" role="presentation"><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div">by <a class="ipc-md-link ipc-md-link--entity" href="/name/nm0295522/?ref_=ttsnd">Cliff Friend</a> and <a class="ipc-md-link ipc-md-link--entity" href="/name/nm0590030/?ref_=ttsnd">Irving Mills</a></div></div><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div">Performed by <a class="ipc-md-link ipc-md-link--entity" href="/name/nm0930729/?ref_=ttsnd">Hank Williams</a></div></div><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div">Courtesy of PolyGram Special Markets</div></div></ul></div></li><li role="presentation" class="ipc-metadata-list__item ipc-metadata-list__item--stacked" data-testid="list-item"><button class="ipc-metadata-list-item__label" role="button" tabindex="0" aria-disabled="false">Willie and The Hand Jive</button><div class="ipc-metadata-list-item__content-container"><ul class="ipc-inline-list ipc-inline-list--show-dividers ipc-inline-list--inline ipc-metadata-list-item__list-content base" role="presentation"><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div">by <a class="ipc-md-link ipc-md-link--entity" href="/name/nm0652906/?ref_=ttsnd">Johnny Otis</a></div></div><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div">Courtesy of Capitol Records</div></div><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div">Under license from CEMA Special Markets</div></div></ul></div></li><li role="presentation" class="ipc-metadata-list__item ipc-metadata-list__item--stacked" data-testid="list-item"><button class="ipc-metadata-list-item__label" role="button" tabindex="0" aria-disabled="false">A Mighty Fortress Is Our God</button><div class="ipc-metadata-list-item__content-container"><ul class="ipc-inline-list ipc-inline-list--show-dividers ipc-inline-list--inline ipc-metadata-list-item__list-content base" role="presentation"><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div">(uncredited)</div></div><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div">Written by <a class="ipc-md-link ipc-md-link--entity" href="/name/nm1051447/?ref_=ttsnd">Martin Luther</a></div></div><div class="ipc-html-content ipc-html-content--base ipc-metadata-list-item-html-item" role="presentation"><div class="ipc-html-content-inner-div">Performed by <a class="ipc-md-link ipc-md-link--entity" href="/name/nm0348409/?ref_=ttsnd">Bob Gunton</a></div></div></ul></div></li></ul></body></html>""";
    private static Soundtrack ValidSoundTrack = new Soundtrack("If I Didn't Care", new List<string>() {
            "Jack Lawrence",
            "The Ink Spots",
            "MCA Records"
        });

    [Theory]
    [AutoMoqData]
    internal async Task SoundtrackParser_Succeeds(
        ILogger<SoundtrackParser> logger,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress,
        string id
        )
    {
        // Arrange
        handler.SetupAnyRequest()
            .ReturnsResponse(ValidHtml);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        var sut = new SoundtrackParser(logger, client);
        // Act

        var result = await sut.ParseAsync(new List<string>() { id }, It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNullOrEmpty();
            result?.FirstOrDefault()?.Should()?.BeEquivalentTo<Soundtrack>(ValidSoundTrack);
        }
    }

    [Theory]
    [AutoMoqData]
    internal async Task SoundtrackParser_Throws_ReturnsNull(
        ILogger<SoundtrackParser> logger,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress,
        string id
        )
    {
        // Arrange
        handler.SetupAnyRequest().ThrowsAsync(new Exception());

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        var sut = new SoundtrackParser(logger, client);
        // Act

        var result = await sut.ParseAsync(new List<string>() { id }, It.IsAny<CancellationToken>());

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeNull();
        }
    }
}
