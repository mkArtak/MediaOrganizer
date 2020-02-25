namespace MediaOrganizer.Core
{
    public class PhysicalFilesOrganizerFactory
    {
        private readonly PhysicalFileMover fileMover = new PhysicalFileMover();

        private readonly PhysicalFileEnumerator enumerator = new PhysicalFileEnumerator();

        public PhysicalFilesOrganizerFactory()
        {
        }

        public IFilesOrganizer Create()
        {
            return new PhysicalFileOrganizer(this.fileMover, this.enumerator);
        }
    }
}
