using System;
using System.Collections.Generic;

public static class PlayerFinder
{
    public static List<MatchMakingTicket> FindPlayers(MatchMakingTicket currentTicket, List<MatchMakingTicket> playerPools, int Amount, bool filterRanking)
    {
        List<MatchMakingTicket> resultPlayers = new List<MatchMakingTicket>();
        List<MatchMakingTicket> similarPlayers = playerPools.FilterGameMode(currentTicket);
        if (filterRanking)
        {
            similarPlayers = similarPlayers.FitlerRank(currentTicket);
        }
        //Limit number of players
        if(similarPlayers.Count > Amount)
        {
            for (int i = 0; i < Amount - 1; i++)
            {
                resultPlayers.Add(similarPlayers[i]);
            }
        }
        else
        {
            resultPlayers = similarPlayers;
        }
        return resultPlayers;
    }

    private static List<MatchMakingTicket> FilterGameMode(this List<MatchMakingTicket> playerPools,MatchMakingTicket currentTicket)
    {
        List<MatchMakingTicket> similarPlayers = new List<MatchMakingTicket>();
        for (int i = 0;i< playerPools.Count; i++)
        {
            MatchMakingTicket ticket = playerPools[i];
            if(ticket.ModeId == currentTicket.ModeId)
            {
                similarPlayers.Add(ticket);
            }
        }
        return similarPlayers;
    }
    public static int TrophyThreshold = 300;
    private static List<MatchMakingTicket> FitlerRank(this List<MatchMakingTicket> playerPools, MatchMakingTicket currentTicket)
    {
        List<MatchMakingTicket> similarPlayers = new List<MatchMakingTicket>();
        for (int i = 0; i < playerPools.Count; i++)
        {
            MatchMakingTicket ticket = playerPools[i];
            if (Math.Abs(ticket.Trophy - currentTicket.Trophy) < TrophyThreshold)
            {
                similarPlayers.Add(ticket);
            }
        }
        return similarPlayers;
    }
}