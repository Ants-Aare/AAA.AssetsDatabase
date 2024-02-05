#if AAA_LOADINGGEN

using AAA.Core.Runtime.Enums;
using AAA.LoadingGen.Runtime;

namespace AAA.AssetsDatabase.Runtime
{
    [LoadingSequence(Include.None)]
    [IncludeLoadingFeature("Assets")]
    public partial class AssetsLoadingSequence
    {
        
    }
}
#endif
