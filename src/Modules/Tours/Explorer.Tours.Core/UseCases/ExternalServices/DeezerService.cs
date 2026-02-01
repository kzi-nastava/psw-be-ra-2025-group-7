using System.Text.Json;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;

namespace Explorer.Tours.Core.UseCases.ExternalServices;

public class DeezerService : IDeezerService
{
    private readonly HttpClient _httpClient;

    // Mapiranje žanrova na Deezer genre IDs
    private static readonly Dictionary<string, int> GENRE_MAPPING = new()
    {
        { "pop", 132 },
        { "rock", 152 },
        { "hip-hop", 116 },
        { "electronic", 106 },
        { "jazz", 129 },
        { "classical", 98 },
        { "latin", 143 },
        { "reggae", 144 },
        { "country", 100 },
        { "blues", 153 },
        { "metal", 464 },
        { "indie", 85 },
        { "folk", 466 },
        { "soul", 165 },
        { "funk", 169 },
        { "disco", 113 },
        { "house", 113 },
        { "techno", 113 },
        { "world", 173 },
        { "alternative", 85 },
        { "r&b", 165 }
    };

    private static readonly Dictionary<string, List<string>> TOP_ARTISTS_BY_GENRE = new()
    {
        { "pop", new() { 
            "Taylor Swift", "Ed Sheeran", "Ariana Grande", "The Weeknd", "Dua Lipa", "Bruno Mars", "Justin Bieber", "Billie Eilish",
            "Harry Styles", "Adele", "Lady Gaga", "Katy Perry", "Rihanna", "Miley Cyrus", "Selena Gomez", "Shawn Mendes",
            "Post Malone", "Charlie Puth", "Demi Lovato", "Sam Smith", "Justin Timberlake", "Maroon 5", "Coldplay", "OneRepublic",
            "Imagine Dragons", "Twenty One Pilots", "BTS", "Blackpink", "Bad Bunny", "J Balvin",
            "Bajaga i Instruktori", "Đorđe Balašević", "Željko Joksimović", "Marija Šerifović", "Zdravko Čolić", "Oliver Dragojević",
            "Massimo Savić", "Gibonni", "Severina", "Jelena Rozga", "Petar Grašo", "Nina Badrić", "Josipa Lisac",
            "Vlado Georgiev", "Aleksandra Radović", "Emina Jahović", "Sara Jo", "Konstrakta", "Teodora"
        } },

        { "rock", new() { 
            "Queen", "Led Zeppelin", "AC/DC", "The Beatles", "The Rolling Stones", "Pink Floyd", "Nirvana", "Foo Fighters",
            "Red Hot Chili Peppers", "Metallica", "U2", "Guns N' Roses", "Aerosmith", "The Who", "Deep Purple", "Black Sabbath",
            "Pearl Jam", "Soundgarden", "Alice in Chains", "The Doors", "Jimi Hendrix", "Eric Clapton", "Lynyrd Skynyrd",
            "Linkin Park", "Green Day", "Muse", "Arctic Monkeys", "The Killers", "Kings of Leon", "The Black Keys", "Royal Blood",
            "Riblja Čorba", "Bijelo Dugme", "Partibrejkers", "Ekatarina Velika", "Van Gogh", "Galija", "Indexi", "Azra",
            "Crvena Jabuka", "Plavi Orkestar", "Prljavo Kazalište", "Divlje Jagode", "Zabranjeno Pušenje", "Električni Orgazam",
            "Disciplina Kičme", "Rambo Amadeus", "Darkwood Dub", "Vampiri", "Block Out", "Obojeni Program", "Bad Copy"
        } },

        { "hip-hop", new() { 
            "Eminem", "Drake", "Kendrick Lamar", "Jay-Z", "Kanye West", "Travis Scott", "Post Malone", "50 Cent",
            "Snoop Dogg", "Dr. Dre", "Tupac", "The Notorious B.I.G.", "Nas", "Lil Wayne", "J. Cole", "Logic",
            "Tyler, The Creator", "A$AP Rocky", "Mac Miller", "Juice WRLD", "XXXTentacion", "Lil Uzi Vert", "21 Savage",
            "Beogradski Sindikat", "Struka", "Marcelo", "Bad Copy", "Smoke Mardeljano", "Wikluh Sky", "Ajs Nigrutin",
            "Rasta", "Fox", "Coby", "Voyage", "Vojko V", "Who See", "Bassivity", "V.I.P.", "Gru", "Sha", "THCF",
            "High5", "Corona", "Mikri Maus", "Edo Maajka", "Frenkie", "Target", "Straight Jackin'"
        } },

        { "electronic", new() { 
            "Daft Punk", "Calvin Harris", "David Guetta", "Avicii", "The Chainsmokers", "Marshmello", "Martin Garrix", "Kygo",
            "Tiësto", "Armin van Buuren", "Afrojack", "Zedd", "Skrillex", "Diplo", "Major Lazer", "Steve Aoki",
            "Deadmau5", "Eric Prydz", "Swedish House Mafia", "Alesso", "Axwell", "Sebastian Ingrosso", "Hardwell",
            "Gramophonedzie", "Kristijan Molnar", "Coeus", "Who See", "SARS", "Darkwood Dub", "Eyesburn",
            "Laibach", "Kanda Kodža i Nebojša", "Repetitor", "Synthetic", "DJ Shone"
        } },

        { "jazz", new() { 
            "Miles Davis", "John Coltrane", "Louis Armstrong", "Ella Fitzgerald", "Duke Ellington", "Billie Holiday", "Chet Baker",
            "Charlie Parker", "Dizzy Gillespie", "Thelonious Monk", "Charles Mingus", "Dave Brubeck", "Bill Evans", "Herbie Hancock",
            "Wynton Marsalis", "Keith Jarrett", "Pat Metheny", "Chick Corea", "Oscar Peterson", "Stan Getz",
            "Duško Gojković", "Boško Petrović", "Stjepko Gut", "Milko Lazar", "Vojislav Simić", "Jovan Maljoković"
        } },

        { "classical", new() { 
            "Ludwig van Beethoven", "Wolfgang Amadeus Mozart", "Johann Sebastian Bach", "Frédéric Chopin", "Antonio Vivaldi",
            "Pyotr Ilyich Tchaikovsky", "Johannes Brahms", "Claude Debussy", "Igor Stravinsky", "Franz Schubert",
            "Robert Schumann", "Felix Mendelssohn", "Giuseppe Verdi", "Richard Wagner", "Gustav Mahler", "Sergei Rachmaninoff",
            "Stevan Mokranjac", "Isidor Bajić", "Stevan Hristić", "Petar Konjović", "Josip Slavenski"
        } },

        { "latin", new() {
            "Bad Bunny", "J Balvin", "Shakira", "Daddy Yankee", "Maluma", "Ozuna", "Karol G", "Rosalía",
            "Nicky Jam", "Anuel AA", "Rauw Alejandro", "Becky G", "Camilo", "Marc Anthony", "Romeo Santos",
            "Luis Fonsi", "Enrique Iglesias", "Ricky Martin", "Jennifer Lopez", "Pitbull"
        } },

        { "reggae", new() {
            "Bob Marley", "Damian Marley", "Sean Paul", "Shaggy", "UB40", "Inner Circle", "Toots and the Maytals",
            "Jimmy Cliff", "Peter Tosh", "Burning Spear", "Steel Pulse", "Third World", "Morgan Heritage",
            "Eyesburn", "Darkwood Dub", "Orthodox Celts", "Pacifiko", "Kultur Shock"
        } },

        { "country", new() {
            "Johnny Cash", "Dolly Parton", "Willie Nelson", "Garth Brooks", "Luke Bryan", "Carrie Underwood", "Blake Shelton",
            "Keith Urban", "Tim McGraw", "Faith Hill", "Shania Twain", "Kenny Chesney", "Brad Paisley", "George Strait",
            "Reba McEntire", "Alan Jackson", "Toby Keith", "Miranda Lambert", "Chris Stapleton", "Zac Brown Band"
        } },

        { "blues", new() {
            "B.B. King", "Muddy Waters", "Robert Johnson", "Eric Clapton", "Stevie Ray Vaughan", "John Lee Hooker",
            "Howlin' Wolf", "Buddy Guy", "Albert King", "Freddie King", "T-Bone Walker", "Lightnin' Hopkins",
            "Elmore James", "Willie Dixon", "Etta James", "Koko Taylor",
            "Blues Trio", "Bjesovi", "Van Gogh"
        } },

        { "metal", new() {
            "Metallica", "Iron Maiden", "Black Sabbath", "Slayer", "Megadeth", "Judas Priest", "Pantera", "System of a Down",
            "Rammstein", "Slipknot", "Tool", "Korn", "Disturbed", "Avenged Sevenfold", "Bullet for My Valentine",
            "Lamb of God", "Gojira", "Opeth", "Mastodon", "Dream Theater", "Nightwish", "Children of Bodom",
            "Alogia", "Atlantida", "Osvajači", "Heller", "Infest", "Devastator", "Sword", "E-Play"
        } },

        { "indie", new() {
            "Arctic Monkeys", "The Strokes", "Tame Impala", "Vampire Weekend", "Florence + The Machine", "Alt-J", "MGMT",
            "Phoenix", "Two Door Cinema Club", "Foster the People", "The XX", "Bon Iver", "Sufjan Stevens",
            "The National", "LCD Soundsystem", "Interpol", "Yeah Yeah Yeahs", "Arcade Fire", "Modest Mouse",
            "Repetitor", "Kanda Kodža i Nebojša", "Laibach", "Obojeni Program", "Kutu Ma", "Consecration",
            "Kal", "Orthodox Celts", "Tram 11", "Atheist Rap", "Superhiks"
        } },

        { "folk", new() {
            "Bob Dylan", "Simon & Garfunkel", "Mumford & Sons", "The Lumineers", "Of Monsters and Men", "Fleet Foxes",
            "Joan Baez", "Pete Seeger", "Woody Guthrie", "Nick Drake", "Iron & Wine", "The Avett Brothers",
            "First Aid Kit", "The Head and the Heart", "Edward Sharpe & The Magnetic Zeros",
            "Šaban Šaulić", "Lepa Brena", "Dragana Mirković", "Ceca", "Halid Bešlić", "Hanka Paldum",
            "Bijelo Dugme", "Indexi", "Goran Bregović", "Emir Kusturica & The No Smoking Orchestra",
            "Jadranka Stojaković", "Arsen Dedić", "Tereza Kesovija"
        } },

        { "soul", new() {
            "Aretha Franklin", "Marvin Gaye", "Otis Redding", "Al Green", "Sam Cooke", "James Brown", "Ray Charles",
            "Stevie Wonder", "Curtis Mayfield", "Wilson Pickett", "Isaac Hayes", "Bill Withers", "Nina Simone",
            "Etta James", "Gladys Knight", "Diana Ross", "The Temptations", "The Four Tops",
            "Del Arno Band", "Who See", "Bebi Dol", "Slađana Milošević"
        } },

        { "funk", new() {
            "James Brown", "Parliament", "Prince", "Earth Wind & Fire", "Sly and the Family Stone", "George Clinton",
            "Funkadelic", "Kool & the Gang", "The Isley Brothers", "War", "Tower of Power", "Ohio Players",
            "Bootsy Collins", "Rick James", "Chic", "Nile Rodgers",
            "Del Arno Band", "Josipa Lisac", "Dado Topić"
        } },

        { "disco", new() {
            "Bee Gees", "ABBA", "Donna Summer", "Chic", "KC and the Sunshine Band", "Gloria Gaynor",
            "Sister Sledge", "Diana Ross", "Village People", "Earth Wind & Fire", "Kool & the Gang",
            "Sylvester", "Boney M", "Hot Chocolate", "The Trammps",
            "Zdravko Čolić", "Oliver Dragojević", "Dado Topić", "Korni Grupa", "Indexi"
        } },

        { "house", new() {
            "Daft Punk", "Calvin Harris", "Swedish House Mafia", "Deadmau5", "Eric Prydz", "Disclosure",
            "Duke Dumont", "Hot Since 82", "CamelPhat", "MK", "Todd Terry", "Masters at Work",
            "Frankie Knuckles", "Larry Heard", "Green Velvet", "Jamie Jones", "Fisher",
            "Gramophonedzie", "Kristijan Molnar", "DJ Shone", "Coeus", "Who See"
        } },

        { "techno", new() {
            "Carl Cox", "Richie Hawtin", "Jeff Mills", "Adam Beyer", "Nina Kraviz", "Charlotte de Witte",
            "Amelie Lens", "Pan-Pot", "Tale of Us", "Sven Väth", "Laurent Garnier", "Maceo Plex",
            "Ben Klock", "Marcel Dettmann", "Dixon", "Âme", "Boris Brejcha", "Paul Kalkbrenner"
        } },

        { "world", new() {
            "Ravi Shankar", "Youssou N'Dour", "Fela Kuti", "Buena Vista Social Club", "Cesária Évora",
            "Ali Farka Touré", "Salif Keita", "Tinariwen", "Bombino", "Angelique Kidjo",
            "Nusrat Fateh Ali Khan", "Gipsy Kings", "Manu Chao", "Gogol Bordello",
            "Goran Bregović", "Emir Kusturica & The No Smoking Orchestra", "Boban Marković Orkestar",
            "Fanfare Ciocărlia", "Šaban Bajramović", "Esma Redžepova"
        } },

        { "alternative", new() {
            "Radiohead", "Nirvana", "The Smashing Pumpkins", "Pearl Jam", "R.E.M.", "The Cure", "Sonic Youth",
            "Pixies", "The White Stripes", "Beck", "Weezer", "Pavement", "My Bloody Valentine",
            "Stone Temple Pilots", "Jane's Addiction", "Soundgarden", "Alice in Chains", "Nine Inch Nails",
            "Partibrejkers", "Disciplina Kičme", "Električni Orgazam", "Atheist Rap", "Darkwood Dub",
            "Eyesburn", "Obojeni Program", "Block Out", "Laibach", "Repetitor", "Superhiks"
        } },

        { "r&b", new() {
            "Beyoncé", "Usher", "Alicia Keys", "The Weeknd", "Frank Ocean", "SZA", "H.E.R.", "Chris Brown",
            "Rihanna", "Ne-Yo", "Trey Songz", "Jhené Aiko", "Miguel", "Anderson .Paak", "Khalid",
            "Bryson Tiller", "PartyNextDoor", "6LACK", "Kehlani", "Summer Walker", "Jazmine Sullivan",
            "Mary J. Blige", "R. Kelly", "TLC", "Destiny's Child"
        } }
    };

    public DeezerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.deezer.com/");
    }

    public async Task<List<PlaylistTrackDto>> GetRecommendations(
        List<string> genres,
        double targetEnergy,
        double targetValence,
        int targetTempo,
        int limit)
    {
        var tracks = new List<PlaylistTrackDto>();
        var tracksPerGenre = (int)Math.Ceiling((double)limit / Math.Min(genres.Count, 3)) + 10;

        foreach (var genre in genres.Take(3))
        {
            var genreTracks = await GetPopularTracksForGenre(genre.ToLower(), tracksPerGenre);
            tracks.AddRange(genreTracks);
        }

        if (tracks.Count == 0)
        {
            return new List<PlaylistTrackDto>();
        }

        return tracks
            .DistinctBy(t => t.SpotifyTrackId)
            .OrderBy(_ => Random.Shared.Next())
            .Take(limit)
            .ToList();
    }

    private async Task<List<PlaylistTrackDto>> GetPopularTracksForGenre(string genre, int limit)
    {
        try
        {
            var tracks = new List<PlaylistTrackDto>();

            if (!TOP_ARTISTS_BY_GENRE.TryGetValue(genre, out var artistNames))
            {
                artistNames = TOP_ARTISTS_BY_GENRE["pop"];
            }

            var shuffledArtists = artistNames.OrderBy(_ => Random.Shared.Next()).Take(5).ToList();

            var tracksPerArtist = Math.Max(3, limit / shuffledArtists.Count);

            foreach (var artistName in shuffledArtists)
            {
                var searchUrl = $"search/artist?q={Uri.EscapeDataString(artistName)}&limit=1";
                var searchResponse = await _httpClient.GetAsync(searchUrl);

                if (!searchResponse.IsSuccessStatusCode)
                    continue;

                var searchContent = await searchResponse.Content.ReadAsStringAsync();
                var searchData = JsonSerializer.Deserialize<DeezerArtistSearchResponse>(searchContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (searchData?.Data == null || searchData.Data.Count == 0)
                    continue;

                var artistId = searchData.Data[0].Id;
                var tracksUrl = $"artist/{artistId}/top?limit={tracksPerArtist}";
                var tracksResponse = await _httpClient.GetAsync(tracksUrl);

                if (!tracksResponse.IsSuccessStatusCode)
                    continue;

                var tracksContent = await tracksResponse.Content.ReadAsStringAsync();
                var tracksData = JsonSerializer.Deserialize<DeezerTracksResponse>(tracksContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (tracksData?.Data == null)
                    continue;

                tracks.AddRange(tracksData.Data.Select(track => new PlaylistTrackDto
                {
                    SpotifyTrackId = track.Id.ToString(),
                    Name = track.Title,
                    Artist = track.Artist?.Name ?? "Unknown Artist",
                    SpotifyUri = track.Link,
                    DurationMs = track.Duration * 1000
                }));

                if (tracks.Count >= limit)
                    break;
            }

            return tracks;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching tracks for genre {genre}: {ex.Message}");
            return new List<PlaylistTrackDto>();
        }
    }

    // JSON Deserializacija
    private class DeezerArtistSearchResponse
    {
        public List<DeezerArtistSearchResult> Data { get; set; }
    }

    private class DeezerArtistSearchResult
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    private class DeezerTracksResponse
    {
        public List<DeezerTrack> Data { get; set; }
    }

    private class DeezerTrack
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public int Duration { get; set; }
        public DeezerArtist Artist { get; set; }
    }

    private class DeezerArtist
    {
        public string Name { get; set; }
    }
}