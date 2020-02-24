namespace MediaOrganizer.Core
{
    public class FilesOrganizerFactory
    {
        public FilesOrganizerFactory()
        {
        }

        public IFilesOrganizer Create()
        {
            return new PhysicalFileOrganizer(new PhysicalFileMover(), new PhysicalFileEnumerator());
        }
    }
}
