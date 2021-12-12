using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Google.Type;
using Grpc.BeyondGreeter.Greetings;
using Grpc.BeyondGreeter.UnitTest.Testing;
using Grpc.Core;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using Xunit;

namespace Grpc.BeyondGreeter.UnitTest.Features.Greetings
{
    public class SayHelloTests : IClassFixture<TestEnvironmentFixture<Program>>
    {
        private readonly TestEnvironmentFixture<Program> _fixture;

        public SayHelloTests(TestEnvironmentFixture<Program> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "GreeterService.SayHello Succeeds")]
        //[InlineData(null)] // will always fail
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("That Conference")]
        public async Task GreeterService_SayHello_Succeeds(string name)
        {
            var request = new HelloRequest {Name = name};
            var sut = new Greeter.GreeterClient(_fixture.GrpcChannel);
            var response = await sut.SayHelloAsync(request);

            var expectedResponse = $"Hello {name}";
            Assert.NotNull(response);
            Assert.Equal(expectedResponse, response.Message);
        }

        [Fact(DisplayName ="Default string null should throw - doesn't")]
        public async Task DefaultStringShouldFail()
        {
            // Expected Failing Test
            var request = new HelloRequest();
            var sut = new Greeter.GreeterClient(_fixture.GrpcChannel);
            Assert.Null(request.Name);
            var exception = 
                await Assert.ThrowsAsync<RpcException>(async () =>
            {
                await sut.SayHelloAsync(request);
            });
        }
        
        [Fact(DisplayName ="Setting String to Null Throws ArgumentNull exception")]
        public void NullStringShouldFail()
        {
            var exception =  Assert.Throws<ArgumentNullException>( () =>
            {
                var request = new HelloRequest {Name = null};
            });
        }

        [Fact(DisplayName = "SayHelloDefaults returns Default Values for Scalars")]
        public async Task DefaultsReturned()
        {
            var request = new HelloDefaultsRequest();
            var sut = new Greeter.GreeterClient(_fixture.GrpcChannel);

            var result = await sut.SayHelloDefaultsAsync(request);
            
            Assert.NotNull(result);
            Assert.False(result.AcceptedTerms);
            Assert.Equal(0, result.Age);
            Assert.Equal(string.Empty, result.Name);
        }

        [Fact(DisplayName = "SayHelloNullables defaults to null values")]
        public async Task NullablesUseNullAsDefault()
        {
            var request = new HelloNullablesRequest();
            var sut = new Greeter.GreeterClient(_fixture.GrpcChannel);

            var result = await sut.SayHelloNullablesAsync(request);

            Assert.NotNull(result);
            Assert.Null(result.AcceptedTerms);
            Assert.Null(result.Age);
            Assert.Null(result.Name);
        }

        [Fact(DisplayName = "SayHelloNullables succeeds if set to null values")]
        public async Task NullablesSetNullSucceeds()
        {
            var request = new HelloNullablesRequest
            {
                AcceptedTerms = null,
                Age = null,
                Name = null,
            };
            var sut = new Greeter.GreeterClient(_fixture.GrpcChannel);

            var result = await sut.SayHelloNullablesAsync(request);

            Assert.NotNull(result);
            Assert.Null(result.AcceptedTerms);
            Assert.Null(result.Age);
            Assert.Null(result.Name);
        }
        
        [Fact(DisplayName = "Optional - Not Set Are not null")]
        public async Task OptionalNotSet()
        {
            var request = new HelloOptionalsRequest();
            var sut = new Greeter.GreeterClient(_fixture.GrpcChannel);
            var result = 
                await sut.SayHelloOptionalsAsync(request);
            Assert.NotNull(result);
            Assert.Equal(0, result.Age);
            Assert.False(result.AcceptedTerms);
            Assert.Equal(string.Empty, result.Name);
        }
        
        [Fact(DisplayName = "Optional - Cant Set properties null")]
        public void OptionalAttemptSetNull()
        {
            var request = new HelloOptionalsRequest();
            var exception =  
                Assert.Throws<ArgumentNullException>( () =>
            {
                request.Name = null;
            });
            // request.Age = null;  int - won't compile
            //request.AcceptedTerms = null; bool - won't compile
        }

        [Fact(DisplayName = "Optional must use ClearXyz")]
        public void OptionalUseClearXyz()
        {
            var request = new HelloOptionalsRequest();
            request.ClearAge();
            request.ClearAcceptedTerms();
            request.ClearName();
            
            Assert.False(request.HasAge);
            Assert.False(request.HasAcceptedTerms);
            Assert.False(request.HasName);
            
            // HOWEVER - Default Values
            Assert.Equal(0, request.Age);
            Assert.False(request.AcceptedTerms);
            Assert.Equal(string.Empty, request.Name);
        }

        [Fact(DisplayName = "Null Nested should throw, but doesn't")]
        public async Task NullNestNoThrow()
        {
            //Expected Test Failure
            var request = new HelloNestedRequest
            {
                Id = "123",
                Name = null
            };
            var sut = new Greeter.GreeterClient(_fixture.GrpcChannel);
            await Assert.ThrowsAsync<RpcException>(async () =>
            {
                var result = await sut.SayHelloNestedAsync(request);
            });
        }
    }
}