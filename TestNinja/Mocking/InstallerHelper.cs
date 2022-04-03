using System.Net;

namespace TestNinja.Mocking
{
    public class InstallerHelper
    {
        private IFileDownloader _fileDownloader;
        private string _setupDestinationFile;

        public InstallerHelper(IFileDownloader fileDownloader)
        {
            _fileDownloader = fileDownloader;
        }

        public bool DownloadInstaller(string customerName, string installerName)
        {
            try
            {
                var url = string.Format("http://example.com/{0}/{1}", customerName, installerName);

                _fileDownloader.DownloadFile(url, _setupDestinationFile);

                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }
    }
}