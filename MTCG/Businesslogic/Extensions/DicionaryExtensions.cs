using System.Reflection;

namespace SWEN.MTCG.BusinessLogic.DictionaryExtensions;

public static class DictionaryExtensions
{
    public static MethodInfo? GetMethodFromResource(this Dictionary<string[], MethodInfo> dict, string[] resource)
    {
        foreach(string[] handlerResource in dict.Keys)
        {
            if(handlerResource.Length != resource.Length)
                continue;

            for(int i = 0; i < handlerResource.Length; ++i)
            {
                if(handlerResource[i] != resource[i] && !handlerResource[i].StartsWith(':'))
                    break;
                
                if(i == handlerResource.Length -1)
                    return dict[handlerResource];
            }            
        }

        return null;
    }
}