using System;
using System.Threading.Tasks;

/* Normal C# example 
async int SomeFunction()
{
    // does some things
    await Task.Delay(5);
    return 42;
}

int number = await SomeFunction();
/* Normal C# example 



/* Unity application example 
class Vector3
{
    public float X;
    public float Y;
    public float Z;
    public void Move();

    public string Username; 
}

async Vector3 ReturnPositionFunction(float x, float y, float z)
{
    const Vector3 newPos = new Vector3(x, y, z);
    await Task.Delay(1);
    return newPos;
}

async void DoSomethingAsynchronously()
{
    const Vector3 currentPosition = await ReturnPositionFunction(15, 0, 15);
    
    await Task.Delay(1);
    
    currentPosition.Move();
}
*/
/* Unity application example */