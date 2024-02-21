namespace IntoTheGrid.Music
{
    public enum SfxEventType
    {
        Undefined,
        HackingCompleted,
        HackingFailed,
        CellClick,
        SkillOpenCellsAround,
        CellHit,
        FlagCell,
        CellHover,
        CantUseSkill,
        LootKeyFound,
        MineCellOpened,
        SkillReadyToUse,

        //Assistant
        AssistantVirusAttack=100,
        AssistantSkillIsReady,
        HackingFailedAssistant,
        HackingCompletedAssistant,
        AssistantSearchUpgraded,
        
        //UI
        UIDefaultButtonClick=200,
        UILobbyStartHackingTarget,
        UILobbyTargetSlotAppeared,
        UIDefaultButtonHover,
    }
}