using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.UriParser;
using TFT.API.Business.Model;
using System.Diagnostics;

namespace TFT.API.Rest
{
    public static class ODataBuilder
    {
        public static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
   
            return builder.GetEdmModel();
        }

        public static Type GetClrType(ODataPath path, IEdmModel model)
        {
            IEdmCollectionType?[] collectionType = new[] { path.FirstSegment.EdmType as IEdmCollectionType };
            IEdmEntityType?[] entityType = new[] { collectionType[0].ElementType.Definition as IEdmEntityType };

            if (entityType == null || entityType[0] == null)
            {
                throw new Exception("public static Type GetClrType(ODataPath path, IEdmModel model)");
            }


            return (model.GetAnnotationValue<ClrTypeAnnotation>(entityType[0])).ClrType;


            //IEdmCollectionType?[] collectionType = new[] { path.FirstSegment.EdmType as IEdmCollectionType, path.LastSegment.EdmType as IEdmCollectionType };
            //IEdmEntityType?[] entityType = new[] { collectionType[0].ElementType.Definition as IEdmEntityType, collectionType[1].ElementType.Definition as IEdmEntityType };

            //if(entityType == null || entityType[0] == null || entityType[1] == null)
            //{
            //    throw new Exception("public static Type GetClrType(ODataPath path, IEdmModel model)");
            //}

            //if (entityType[0].Equals(entityType[1]) == false)
            //{
            //    throw new NotImplementedException("entityType[0].Equals(entityType[1]) == false");
            //}

            //return (model.GetAnnotationValue<ClrTypeAnnotation>(entityType[0]) ?? model.GetAnnotationValue<ClrTypeAnnotation>(entityType[1])).ClrType;
        }
    }
}
