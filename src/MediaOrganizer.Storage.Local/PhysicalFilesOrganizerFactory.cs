using MediaOrganizer.Core;

namespace MediaOrganizer.Storage.Local
{
    public class PhysicalFilesOrganizerFactory
    {
        private readonly PhysicalFileMover fileMover = new PhysicalFileMover();

        private readonly PhysicalFileEnumerator enumerator = new PhysicalFileEnumerator();

        private readonly Mapper mapper = new Mapper();

        public PhysicalFilesOrganizerFactory()
        {
        }

        public IFilesOrganizer Create()
        {
            return new PhysicalFileOrganizer(this.fileMover, this.enumerator, this.mapper);
        }
    }
}
