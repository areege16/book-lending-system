namespace BookLending.Application.Setting
{
    public class CloudinarySetting
    {
        public string Cloud { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".webp" };
        public long MaxFileSizeInMB { get; set; } = 10;
    }
}