using System.Threading.Tasks;
using Grpc.BeyondGreeter.Members;
using Grpc.BeyondGreeter.UnitTest.Testing;
using Grpc.Core;
using Xunit;

namespace Grpc.BeyondGreeter.UnitTest.Features.Members
{
    public class CreateProfileTests : IClassFixture<TestEnvironmentFixture<Program>>
    {
        private readonly TestEnvironmentFixture<Program> _fixture;

        public CreateProfileTests(TestEnvironmentFixture<Program> fixture)
        {
            _fixture = fixture;
        }
        
        [Fact(DisplayName = "CreateProfile Fails with invalid data")]

        //Usually cover all scenarios
        public async Task MembershipService_CreateProfile_Fails_With_Null_Name()
        {
            // Normally use function to build based on input
            var expectedTrailers = new Metadata()
            {
                {"Email", "Email is required."},
                {"FirstName", "First Name is required."},
                {"LastName", "Last Name is required." },
                {"AcceptedTerms", "Terms must be accepted."}
            };
            var request = new CreateProfileRequest()
            {
                AcceptedTerms = false,
                Age = 48,
            };
            var sut = new Membership.MembershipClient(_fixture.GrpcChannel);

            var ex = await Assert.ThrowsAsync<RpcException>(async () =>
            {
                await sut.CreateProfileAsync(request);
            });
            
            Assert.Equal(StatusCode.InvalidArgument, ex.Status.StatusCode);
            Assert.Equal("Invalid Arguments", ex.Status.Detail);
            Assert.Equal(expectedTrailers, ex.Trailers, new MetaDataComparer());
        }
    }
    
}