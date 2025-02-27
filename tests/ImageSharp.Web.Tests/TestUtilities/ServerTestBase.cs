// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Formats;
using Xunit;
using Xunit.Abstractions;

namespace SixLabors.ImageSharp.Web.Tests.TestUtilities
{
    [Collection("Server tests")]
    public abstract class ServerTestBase<TFixture> : IClassFixture<TFixture>
        where TFixture : TestServerFixture
    {
        private const int Width = 20;

        protected ServerTestBase(TFixture fixture, ITestOutputHelper outputHelper, string imageSource)
        {
            this.Fixture = fixture;
            this.HttpClient = fixture.HttpClient;
            this.OutputHelper = outputHelper;
            this.ImageSource = imageSource;

            this.OutputHelper.WriteLine("EnvironmentalVariables");
            foreach (DictionaryEntry item in Environment.GetEnvironmentVariables())
            {
                this.OutputHelper.WriteLine($"Key = {item.Key}, Value = {item.Value}");
            }
        }

        public TFixture Fixture { get; }

        public HttpClient HttpClient { get; }

        public ITestOutputHelper OutputHelper { get; }

        public string ImageSource { get; }

        [Fact]
        public async Task CanProcessAndResolveImageAsync()
        {
            string url = this.ImageSource;

            string ext = Path.GetExtension(url);
            IImageFormat format = Configuration.Default.ImageFormatsManager.FindFormatByFileExtension(ext);

            // First response
            HttpResponseMessage response = await this.HttpClient.GetAsync(url + await this.AugmentCommandAsync(this.Fixture.Commands[0]));

            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
            Assert.True(response.Content.Headers.ContentLength > 0);
            Assert.Equal(format.DefaultMimeType, response.Content.Headers.ContentType.MediaType);

            (Image Image, IImageFormat Format) actual = await Image.LoadWithFormatAsync(await response.Content.ReadAsStreamAsync());
            using Image image = actual.Image;

            Assert.Equal(Width, image.Width);
            Assert.Equal(format, actual.Format);

            response.Dispose();

            // Cached Response
            response = await this.HttpClient.GetAsync(url + await this.AugmentCommandAsync(this.Fixture.Commands[0]));

            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
            Assert.True(response.Content.Headers.ContentLength > 0);
            Assert.Equal(format.DefaultMimeType, response.Content.Headers.ContentType.MediaType);

            (Image Image, IImageFormat Format) cachedActual = await Image.LoadWithFormatAsync(await response.Content.ReadAsStreamAsync());
            using Image cached = cachedActual.Image;

            Assert.Equal(Width, cached.Width);
            Assert.Equal(format, actual.Format);

            response.Dispose();

            // 304 response
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url + await this.AugmentCommandAsync(this.Fixture.Commands[0])),
                Method = HttpMethod.Get,
            };

            request.Headers.IfModifiedSince = DateTimeOffset.UtcNow;

            response = await this.HttpClient.SendAsync(request);

            // This test is flaky in the CI environment.
            if (!TestEnvironment.RunsOnCI)
            {
                Assert.Equal(HttpStatusCode.NotModified, response.StatusCode);
                Assert.Equal(0, response.Content.Headers.ContentLength);
            }

            Assert.Equal(format.DefaultMimeType, response.Content.Headers.ContentType.MediaType);

            request.Dispose();
            response.Dispose();

            // 412 response
            request = new HttpRequestMessage
            {
                RequestUri = new Uri(url + await this.AugmentCommandAsync(this.Fixture.Commands[0])),
                Method = HttpMethod.Get,
            };

            request.Headers.IfUnmodifiedSince = DateTimeOffset.MinValue;

            response = await this.HttpClient.SendAsync(request);

            Assert.Equal(HttpStatusCode.PreconditionFailed, response.StatusCode);
            Assert.Equal(0, response.Content.Headers.ContentLength);

            request.Dispose();
            response.Dispose();
        }

        [Fact]
        public async Task CanProcessMultipleIdenticalQueriesAsync()
        {
            string url = this.ImageSource;
            string command1 = await this.AugmentCommandAsync(this.Fixture.Commands[0]);
            string command2 = await this.AugmentCommandAsync(this.Fixture.Commands[1]);

            Task[] tasks = Enumerable.Range(0, 100).Select(i => Task.Run(async () =>
            {
                string command = i % 2 == 0 ? command1 : command2;
                using HttpResponseMessage response = await this.HttpClient.GetAsync(url + command);
                Assert.NotNull(response);
                Assert.True(response.IsSuccessStatusCode);
                Assert.True(response.Content.Headers.ContentLength > 0);
            })).ToArray();

            var all = Task.WhenAll(tasks);
            await all;
            Assert.True(all.IsCompletedSuccessfully);
        }

        protected virtual Task<string> AugmentCommandAsync(string command) => Task.FromResult(command);
    }
}
