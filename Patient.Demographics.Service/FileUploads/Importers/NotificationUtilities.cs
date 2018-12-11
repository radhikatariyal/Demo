namespace Patient.Demographics.Service.FileUploads.Importers
{
    public static class NotificationUtilities
    {
        public static int CalculateNotifierRate(int totalNumberOfRows)
        {
            var numberOfRowsToSendProgressUpdate = totalNumberOfRows / 100;

            if (numberOfRowsToSendProgressUpdate < 100)
            {
                numberOfRowsToSendProgressUpdate = 100;
            }
            else if (numberOfRowsToSendProgressUpdate > 500)
            {
                numberOfRowsToSendProgressUpdate = 500;
            }

            return numberOfRowsToSendProgressUpdate;
        }
    }
}