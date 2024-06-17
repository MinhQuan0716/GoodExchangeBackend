using Application.ViewModel.UserViewModel;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.SchemaFilter
{
    public class RegisterSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
           if(context.Type== typeof(RegisterModel))
            {
                schema.Example = new OpenApiObject
                {
                    ["Username"] = new OpenApiString("string"),
                    ["Email"] = new OpenApiString("string"),
                    ["Password"] = new OpenApiString("string"),
                    ["Fullname"] = new OpenApiString("string"),
                    ["Birthday"] = new OpenApiString(DateTime.UtcNow.ToString("yyyy-MM-dd")),
                    ["Phonenumber"] = new OpenApiString("0933125677"),
                    ["Homeaddress"]=new OpenApiString("string")
                };
            }
        }
    }
}
