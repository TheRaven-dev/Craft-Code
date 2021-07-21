using wManager.Wow.Helpers;
using wManager.Wow.Class;
using Quester.Profile;
using System;

//Place into Quester > CustomScripts.
public static class QuestHelper
{
    public static void CreateQuesterStep(QuestAction questAction, String Code)
    {
        var p = Quest.QuesterCurrentContext.Profile as QuesterProfile;
        if (p != null)
        {
            p.QuestsSorted.Add(new QuestsSorted { Action = questAction, NameClass = Code });
        }
    }
    public static void ExecuteStep(String StepName, int BeforeStep = 0)
    {
        var p = Quest.QuesterCurrentContext.Profile as QuesterProfile;
        if (p != null)
        {
            for (int i = 0; i < p.QuestsSorted.Count; i++)
            {
                if (p.QuestsSorted[i].NameClass == StepName)
                {
                    Quest.QuesterCurrentContext.CurrentStep = i + BeforeStep;
                    break;
                }
            }
        }
    }
}

//Creates the Load profile Step. (Will be last step in a profile.)
//List of Quest Actions - https://wrobot.eu/byme/doc/html/T-wManager.Wow.Class.QuestAction.htm
//Code mean the code you want to use, like if set to lua, code = You're lua Code.
//QuestHelper.CreateQuesterStep(QuestAction.LoadProfile, "LoadedProfile.xml");

//will jump (excute) the step name that is listed.
//QuestHelper.LoadStep("LoadedProfile.xml");
