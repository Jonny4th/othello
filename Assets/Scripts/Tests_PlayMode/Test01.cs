using System;
using System.Collections;
using UnityEngine.TestTools;

public class Test01
{
    [UnityTest]
    public IEnumerator Test01WithEnumeratorPasses()
    {
        yield return null;
    }
}
