using Dalamud.Game.ClientState.Party;

namespace SamplePlugin.Utils;

public class DalamudUtils
{
    private PartyList partyList;

    public DalamudUtils(PartyList partyList)
    {
        this.partyList = partyList;
    }

    public bool IsGrouped()
    {
        if (partyList.Length == 0)
        {
            return false;
        }
        return true;
    }

    public int GetGroupMembersCount()
    {
        return partyList.Length;
    }
}
