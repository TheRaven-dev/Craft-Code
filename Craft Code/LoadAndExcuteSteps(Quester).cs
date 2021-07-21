//Place into Quester > CustomScripts.

public static class Load
{
    public static void Profile(string ProfileName)
    {
        var p = wManager.Wow.Helpers.Quest.QuesterCurrentContext.Profile as Quester.Profile.QuesterProfile;
        if (p != null)
        {
            p.QuestsSorted.Add(new wManager.Wow.Class.QuestsSorted { Action = wManager.Wow.Class.QuestAction.LoadProfile, NameClass = ProfileName + ".xml" });
        }
    }

    public static void LoadStep(string stepName)
    {
        var p = wManager.Wow.Helpers.Quest.QuesterCurrentContext.Profile as Quester.Profile.QuesterProfile;
        if (p != null)
        {
            for (int i = 0; i < p.QuestsSorted.Count; i++)
            {
                if (p.QuestsSorted[i].NameClass == stepName + ".xml")
                {
                    wManager.Wow.Helpers.Quest.QuesterCurrentContext.CurrentStep = i;
                    break;
                }
            }
        }
    }
}

//Creates the Load profile Step. (Will be last step in a profile.)
//Load.Profile("LoadedProfile");

//will jump (excute) the step name that is listed.
//Load.LoadStep("LoadedProfile");
