using Explorer.BuildingBlocks.Tests;

namespace Explorer.Notes.Tests
{
    public class BaseNotesIntegrationTest : BaseWebIntegrationTest<NotesTestFactory>
    {
        public BaseNotesIntegrationTest(NotesTestFactory factory) : base(factory) { }
    }
}