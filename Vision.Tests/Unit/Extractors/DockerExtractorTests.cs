namespace Vision.Tests
{
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Vision.Web.Core;
    using Xunit;

    public class DockerExtractorTests
    {
        private readonly DockerAssetExtractor sut;

        public DockerExtractorTests()
        {
            sut = new DockerAssetExtractor(Substitute.For<ILogger<DockerAssetExtractor>>());
        }
                
        [Theory]
        [InlineData("FROM ubuntu", "ubuntu", "")]
        [InlineData("FROM ubuntu:latest", "ubuntu", "latest")]
        [InlineData("FROM golang:1.7.3 as builder", "golang", "1.7.3")]
        public void ExtractImage(string file, string image, string version)
        {
            var dependencies = sut.ExtractDependencies(new Asset { Raw = file }).ToList();
            
            Assert.Equal(image, dependencies[0].Name);
            Assert.Equal(version, dependencies[0].Version);
        }        

        [Theory]
        [MemberData(nameof(InlineImages))]
        public void ExtractMultiImages(string file, string[] images, string[] versions)
        {            
            var dependencies = sut.ExtractDependencies(new Asset { Raw = file }).ToList();

            for(int i = 0; i < dependencies.Count; i++)
            {
                Assert.Equal(images[i], dependencies[i].Name);
                Assert.Equal(versions[i], dependencies[i].Version);
            }

            Assert.Equal(images.Length, dependencies.Count);
        }

        public static IEnumerable<object[]> InlineImages => new List<object[]>
        {
            new object[]
            {
                "FROM ubuntu:latest" + Environment.NewLine + "FROM microsoft/dotnet:sdk" + Environment.NewLine + "FROM microsoft/dotnet:runtime",
                new[] { "ubuntu", "microsoft/dotnet", "microsoft/dotnet" },
                new[] { "latest", "sdk", "runtime" }
            },
            new object[]
            {
                "FROM ubuntu:latest as ubuilder" + Environment.NewLine + "FROM microsoft/dotnet:sdk as sdkBuilder" + Environment.NewLine + "FROM microsoft/dotnet:runtime as runtimeBuilder",
                new[] { "ubuntu", "microsoft/dotnet", "microsoft/dotnet" },
                new[] { "latest", "sdk", "runtime" }
            },
            new object[]
            {
                "FROM custom/aspnetcore-build:latest AS builder" + Environment.NewLine + "FROM builder as build" + Environment.NewLine + "FROM builder as publish" + Environment.NewLine + "FROM custom/aspnetcore:latest",
                new[] { "custom/aspnetcore-build", "custom/aspnetcore" },
                new[] { "latest", "latest" }
            }
        };
    }
}
