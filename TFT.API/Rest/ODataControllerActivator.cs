using Microsoft.AspNetCore.OData.Routing.Conventions;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.AspNetCore.OData.Routing;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.AspNetCore.OData.Extensions;

namespace TFT.API.Rest
{
    public interface IODataControllerActivator : IODataControllerActionConvention
    {
    }

    public class ODataControllerActivator : ODataSegmentTemplate, IODataControllerActivator
    {

        public Int32 Order => 0;
        private readonly IEnumerable<Type>? _predefinedControllers;

        protected IControllerDomain? controllerDomain;

        public ODataControllerActivator()
        {
            _predefinedControllers = null;
            controllerDomain = null;
        }

        //public ODataControllerActivator(IControllerDomain domain)
        //{
        //    _predefinedControllers = new Type[] { typeof(ODataCrudController) };
        //    controllerDomain = domain;
        //}

        public Boolean AppliesToAction(ODataControllerActionContext context)
        {
            if (_predefinedControllers.Contains(context.Controller.ControllerType.UnderlyingSystemType) && (new[] { "Get", "Post", "Patch", "Delete" }.Contains(context.Action.ActionName)))
            {
                context.Action.AddSelector(context.Action.ActionName, context.Prefix, context.Model, new ODataPathTemplate(this));
                return true;
            }

            return false;
        }

        public Boolean AppliesToController(ODataControllerActionContext context)
        {
            Boolean selectedController = _predefinedControllers.Union(controllerDomain.ControllerTypes).Contains(context.Controller.ControllerType.AsType());
            return selectedController;
        }

        public override IEnumerable<String> GetTemplates(ODataRouteOptions options)
        {
            yield return "/{resource}({key})";
            yield return "/{resource}";
        }

        public override bool TryTranslate(ODataTemplateTranslateContext context)
        {
            context.RouteValues.TryGetValue("resource", out Object resource);
            String entitySetName = resource as String;

            IEdmEntitySet edmEntitySet = context.Model.EntityContainer.EntitySets()
                .FirstOrDefault(e => string.Equals(entitySetName, e.Name, StringComparison.OrdinalIgnoreCase));

            if (edmEntitySet != null)
            {
                context.Segments.Add(new EntitySetSegment(edmEntitySet));
                return true;
            }

            return false;
        }

    }
}
