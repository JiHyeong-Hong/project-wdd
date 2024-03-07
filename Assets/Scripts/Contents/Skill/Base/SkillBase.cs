using Data;

public abstract class SkillBase
{
    public Creature Owner { get; protected set; }
    public SkillData SkillData { get; private set; }

    public void SetData(SkillData data)
    {
        SkillData = data;
    }
    
    public virtual void Init(Creature owner,SkillData data = null)
    {
        Owner = owner;

        if (data != null)
            SkillData = data;
    }
    
    public virtual void LevelUp(SkillData data)
    {
        SetData(data);
    }
}
