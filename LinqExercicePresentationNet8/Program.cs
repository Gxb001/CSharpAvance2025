using DataSources;

//dataSources
var allAlbums = ListAlbumsData.ListAlbums;
var allArtists = ListArtistsData.ListArtists;

// decommenter les exercices un par un


// Ex : 1
// // Afficher tous les albums dans le terminal avec leur numéro
// // Select permet de extraire des informations d'un tableau ou d'une collection. 
// // ToList permet de convertir une collection en liste.
// // ForEach permet d'itérer sur chaque élément de la liste
// allAlbums.Select((album, index) => $"Album N°{index + 1} : {album.Title}").ToList().ForEach(Console.WriteLine);

// Ex : 2
// // Demander à l'utilisateur de donner le titre un text ex "out" et renvoie tous les albums contenant ce texte dans leur titre
// // Where permet d'appliquer une condition sur chaque élément d'une collection.
// Console.WriteLine("Entrez une chaine de caracteres : ");
// String content = Console.ReadLine();
// var filteredAlbums = allAlbums.Where(album => album.Title.Contains(content)).ToList();
//
// // Afficher tous les albums filtrés dans le terminal avec leur numéro
// filteredAlbums.Select((album, index) => $"Album N°{index + 1} : {album.Title}").ToList().ForEach(Console.WriteLine);

// // Ex : 3
// // demander à l'utilisateur de donner un titre ex "out" et renvoie tous les albums contenant ce texte dans leur titre
// Console.WriteLine("Entrez une chaine de caracteres : ");
// String content = Console.ReadLine();
// var filteredAlbums = allAlbums.Where(album => album.Title.Contains(content)).ToList();
//
// // trier la liste dans l'ordre ascending puis descending
// // utilisation de OrderBy & ThenBy - Correction: Sort ascending and print, then sort descending and print
// var ascendingAlbums = filteredAlbums.OrderBy(album => album.Title).ThenBy(album => album.AlbumId).ToList();
// Console.WriteLine("Albums triés par titre et artiste ascendant :");
// ascendingAlbums.Select((album, index) => $"Album N°{index + 1} : {album.Title}").ToList().ForEach(Console.WriteLine);
//
// var descendingAlbums = filteredAlbums.OrderByDescending(album => album.Title).ThenByDescending(album => album.AlbumId).ToList();
// Console.WriteLine("Albums triés par titre et artiste descendant :");
// descendingAlbums.Select((album, index) => $"Album N°{index + 1} : {album.Title}").ToList().ForEach(Console.WriteLine);

// // Ex : 4 
// // afficher le résultat de recherche groupé par artiste
// // format  de allAlbums : new Album(AlbumId, Title,ArtistId)
//
// Console.WriteLine("Entrez une chaine de caracteres : ");
// String content = Console.ReadLine();
//
// var groupedAlbums = from album in allAlbums
//     where album.Title.Contains(content)
//     group album by album.ArtistId into artistGroup
//     orderby artistGroup.Key
//     select artistGroup;
//
// // foreach (var group in groupedAlbums)
// // {
// //     Console.WriteLine($"Artiste ID {group.Key}:");
// //     foreach (var album in group)
// //     {
// //         Console.WriteLine($"  - {album.Title}");
// //     }
// // }
//
// // ou
//
// groupedAlbums
//     .Select(group =>
//         $"Artiste ID {group.Key}:\n" +
//         string.Join("\n", group.Select(album => $"  - {album.Title}"))
//     )
//     .ToList()
//     .ForEach(Console.WriteLine);

// // Ex : 5
// Console.WriteLine("Entrez une chaine de caracteres : ");
// String content = Console.ReadLine();
//
// var groupedAlbums = from album in allAlbums
//     join artist in allArtists on album.ArtistId equals artist.ArtistId
//     where album.Title.Contains(content)
//     group album by artist.Name into artistGroup
//     orderby artistGroup.Key
//     select artistGroup;
//
// // Afficher les albums groupés par artiste
// foreach (var group in groupedAlbums)
// {
//     Console.WriteLine($"Artiste : {group.Key}");
//     foreach (var album in group)
//     {
//         Console.WriteLine($"  - Album N°{album.AlbumId} : {album.Title}");
//     }
// }

// // Ex : 6
// var albumsForDisplay = 
//     from album in allAlbums 
//     let affichageAlbum = $"   Album n°{album.AlbumId} : {album.Title}" orderby album.AlbumId select affichageAlbum;
//
// var albumsList = albumsForDisplay.ToList();
// int pageSize = 20;
// int totalPages = (int)Math.Ceiling((double)albumsList.Count / pageSize);
//
// Console.WriteLine($"Total d'albums : {albumsList.Count}, Pages totales : {totalPages}");
// Console.WriteLine("Entrez le numéro de page (commençant par 1) : ");
// if (int.TryParse(Console.ReadLine(), out int page) && page >= 1 && page <= totalPages)
// {
//     var pageAlbums = albumsList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
//     Console.WriteLine($"Page {page} :");
//     pageAlbums.ForEach(Console.WriteLine);
// }
// else
// {
//     Console.WriteLine("Numéro de page invalide.");
// }

// // Ex : 7 
// // Lire un fichier txt
// Console.WriteLine("Entrez une chaîne de caractères : ");
// string content = Console.ReadLine();
//
// foreach (var line in File.ReadAllLines($@"{Directory.GetCurrentDirectory()}/Text/Albums.txt")
//              .Where(line => string.IsNullOrEmpty(content) || line.Contains(content, StringComparison.OrdinalIgnoreCase)) 
//              .OrderBy(line => line))
// {
//     Console.WriteLine(line);
// }  

// //Ex : 8 
// // transformer allArtists en XML
// Console.WriteLine("Conversion en XML");
// var xml = new XDocument(
//     new XElement("Albums",
//         from artist in allArtists
//         select new XElement("Artist", new XElement("Name", artist.Name), new XElement("ID", artist.ArtistId))));
// Console.WriteLine(xml);