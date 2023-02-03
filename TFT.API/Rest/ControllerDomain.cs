namespace TFT.API.Rest
{
    public interface IControllerDomain
    {
        public IEnumerable<Type> ControllerTypes { get; }
    }
    public class ControllerDomain : IControllerDomain
    {
        public ControllerDomain(IEnumerable<Type> controllerTypes)
        {
            ControllerTypes = controllerTypes;
        }

        public IEnumerable<Type> ControllerTypes { get; private set; }
    }
}
