using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Farseer;

namespace FirstGame.Game;

public class GameEntity : Entity
{
    public enum Friendliness {
        Neutral,
        Friendly,
        Hostile
    }
    public enum Kind {
        Zombie,
        Player
    }
    private int _hp = 1;
    internal virtual int Hp {
        set { 
            _hp = Math.Max(0, value);
            if (_hp == 0)
                IsAlive = false;
        }

        get { return _hp; }
    }
    private int _maxHp = 1;
    internal virtual int MaxHp
    {
        get { return _maxHp; }
        set
        {
            _maxHp = Math.Max(0, value);
            if (_maxHp == 0)
                IsAlive = false;
        }
    }
    internal virtual bool IsAlive { get; set; } = true;
    internal virtual FSRigidBody Body { get; set; }
    internal virtual Friendliness EntityFriendliness { get; set; } = Friendliness.Neutral;
    internal virtual Kind EntityKind { get; set; }
    public GameEntity() : base()
    {
    }
    public GameEntity(string name) : base(name)
    {
        
    }
    public virtual int Hurt(int damage){
        Hp = Math.Max(0, Hp-damage);
        if (Hp == 0)
            Kill();
        
        return Hp;
    }
    // public Vector2? GetPosition(){
    //     if (Body != null)
    //         return Body.Position;
    //     else
    //         return null;
    // }
    // public void SetPosition(Vector2 pos)
    // {
    //     FarseerPhysics.Common.Transform trf;
    //     Body.GetTransform(out trf);
    //     Body.SetTransform(pos, trf.Q.GetAngle());
    // }
    public virtual string GetName() {
        return EntityFriendliness + " Entity";
    }
    public virtual void Kill(){
        IsAlive = false;
    }
}