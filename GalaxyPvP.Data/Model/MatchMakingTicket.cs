
public class MatchMakingTicket
{
    public string PlayerId { get; set; }

    public string ClientVersion { get; set; }
    public string Region { get; set; }
    public int ModeId { get; set; }
    public int Trophy {  get; set; }
    public bool IsHost { get; set; } = false;

    public DateTime SubmitedTime { get; set; }

}