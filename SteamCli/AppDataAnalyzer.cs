using System.Net;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace SteamCli;

public class AppDataAnalyzer
{
    private App applist;
    public void AnalizeJson()
    {
        if (File.Exists("gamelistdata.json"))
        {
            applist = JsonConvert.DeserializeObject<App>(File.ReadAllText("gamelistdata.json"));
            Console.WriteLine("Successfully reloaded list of steam games");
        }
        else
        {
            Console.WriteLine("No gamelistdata.json file found, do you want to refresh it ? [Y/N]");
            string resp = Console.ReadLine();
            if (resp.Contains("Y") || resp.Contains("y"))
            {
                RefreshGameList();
            }
            else
            {
                Console.WriteLine("List not updated, user declined mandatory update of gamelistdata.json");
                return;
            }
        }
    }
    public void RefreshGameList()
    {
        try
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFile("https://api.steampowered.com/ISteamApps/GetAppList/v2/", "gamelistdata.json");
            applist = JsonConvert.DeserializeObject<App>(File.ReadAllText("gamelistdata.json"));
        }
        catch(Exception e)
        {
            Console.WriteLine("Failure could not refresh game list");
            Console.Error.NewLine = e.Message;
            return;
        }
        Console.WriteLine("Successfully refreshed game list");
        System.GC.Collect();
    }

    public Game[] FindMatch(string searchedPhrase)
    {
        return SearchGameList.SearchBy(applist.applist.apps, searchedPhrase);
    }
}

public class Applist
{
    public List<Game> apps;
}
public class Game : IComparable<Game>
{
    [JsonProperty(PropertyName = "name")]public string name;
    public int appid;
    public int matchValue;

    public int CompareTo(Game other)
    {
        if (null == other)
            return 1;
        // string.Compare is safe when Id is null 
        return this.matchValue.CompareTo(other.matchValue);
    }
}

public class App
{
    public Applist applist;
}