using System;
using Topsis.Domain.Contracts;

namespace Topsis.AcceptanceTests.TestServer
{
    [Flags]
    public enum Scenario
    {
        None = 0,
        Workspace_Create = 1,
        Workspace_CalculateResults = 2
    }

    public class TestContext
    {
        private readonly Type _testType;

        public TestUserContext UserContext { get; set; }

        public TestContext(Type testType, Scenario scenario, string userId = "test-user-id")
        {
            Id = Guid.NewGuid();
            Scenario = scenario;
            _testType = testType;

            UserContext = new TestUserContext(userId);
        }

        public Guid Id { get; }
        public Scenario Scenario { get; set; }


        public string BuildScenarioFileName()
        {
            return $"{(int)Scenario}_{_testType.Name}";
        }
    }

    public class TestUserContext : IUserContext
    {
        public TestUserContext(string userId = "test-user-id")
        {
            UserId = userId;
        }

        public string UserId { get; set; }

        internal void SwitchUser(string userId)
        {
            UserId = userId;
        }
    }
}
