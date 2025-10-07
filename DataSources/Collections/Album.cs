namespace DataSources;

public class Album
{
    public Album(int albumId, string title, int artistId)
    {
        AlbumId = albumId;
        Title = title;
        ArtistId = artistId;
    }

    public int AlbumId { get; set; }
    public string Title { get; set; }

    public int ArtistId { get; set; }
}