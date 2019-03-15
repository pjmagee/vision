namespace Vision.Tests
{
    using System;
    using System.Linq;
    using Vision.Web.Core;
    using Xunit;

    public class DockerExtractorTests
    {
        private readonly DockerAssetExtractor sut;

        public DockerExtractorTests()
        {
            this.sut = new DockerAssetExtractor();
        }

        [Fact]
        public void ExtractsImageFromSimpleDockerfile()
        {
            // arrange
            var file = "FROM ubuntu";

            // act
            var dependencies = sut.ExtractDependencies(new Asset { Id = Guid.NewGuid(), Raw = file }).ToList();

            // assert
            Assert.Equal("ubuntu", dependencies[0].Name);
            Assert.Equal(string.Empty, dependencies[0].Version);
        }

        [Fact]
        public void ExtractImageAndTagFromDockerfile()
        {
            // arrange
            var file = "FROM ubuntu:latest";

            // act
            var dependencies = sut.ExtractDependencies(new Asset { Id = Guid.NewGuid(), Raw = file }).ToList();

            Assert.Equal("ubuntu", dependencies[0].Name);
            Assert.Equal("latest", dependencies[0].Version);
        }

        [Fact]
        public void ExtractImageAndTagFromAsBuilder()
        {
            // arrange
            var file = "FROM golang:1.7.3 as builder";

            // act
            var dependencies = sut.ExtractDependencies(new Asset { Id = Guid.NewGuid(), Raw = file }).ToList();

            Assert.Equal("golang", dependencies[0].Name);
            Assert.Equal("1.7.3", dependencies[0].Version);
        }

        [Fact]
        public void ExtractMultipleImagesAndTagsFromDockerfile()
        {
            // arrange
            var file = "FROM ubuntu:latest" + Environment.NewLine + 
                       "FROM microsoft/dotnet:sdk" + Environment.NewLine + 
                       "FROM microsoft/dotnet:runtime";

            // act
            var dependencies = sut.ExtractDependencies(new Asset { Id = Guid.NewGuid(), Raw = file }).ToList();

            Assert.Equal("ubuntu", dependencies[0].Name);
            Assert.Equal("latest", dependencies[0].Version);

            Assert.Equal("microsoft/dotnet", dependencies[1].Name);
            Assert.Equal("sdk", dependencies[1].Version);

            Assert.Equal("microsoft/dotnet", dependencies[2].Name);
            Assert.Equal("runtime", dependencies[2].Version);
        }

        [Fact]
        public void ExtractMultipleImagesAndTagsFromAsBuilders()
        {
            // arrange
            var file = "FROM ubuntu:latest as ubuilder" + Environment.NewLine + 
                       "FROM microsoft/dotnet:sdk as sdkBuilder" + Environment.NewLine + 
                       "FROM microsoft/dotnet:runtime as runtimeBuilder";

            // act
            var dependencies = sut.ExtractDependencies(new Asset { Id = Guid.NewGuid(), Raw = file }).ToList();

            Assert.Equal("ubuntu", dependencies[0].Name);
            Assert.Equal("latest", dependencies[0].Version);

            Assert.Equal("microsoft/dotnet", dependencies[1].Name);
            Assert.Equal("sdk", dependencies[1].Version);

            Assert.Equal("microsoft/dotnet", dependencies[2].Name);
            Assert.Equal("runtime", dependencies[2].Version);
        }

        [Fact]
        public void ExtractMultipleImagesAndFilterAsBuilders()
        {
            // arrange
            var file = "FROM xperthr/aspnetcore-build:latest AS builder" + Environment.NewLine +
                       "FROM builder as build" + Environment.NewLine + 
                       "FROM builder as publish" + Environment.NewLine + 
                       "FROM xperthr/aspnetcore:latest";
            // act
            var dependencies = sut.ExtractDependencies(new Asset { Id = Guid.NewGuid(), Raw = file }).ToList();

            Assert.Equal(2, dependencies.Count);

            Assert.Equal("xperthr/aspnetcore-build", dependencies[0].Name);
            Assert.Equal("latest", dependencies[0].Version);

            Assert.Equal("xperthr/aspnetcore", dependencies[1].Name);
            Assert.Equal("latest", dependencies[1].Version);
        }
    }
}
