using FirstGame.Game.objects.bodies;
using FirstGame.Game.objects.bodies.player;

namespace FirstGame.Game.objects
{
    internal interface IInteractable
    {
        public void Interact(Player player);
    }

}
