using System.Collections;
using System.Collections.Generic;
using EMU6502;
using UnityEngine;

public class RAM_Interface : MonoBehaviourSingleton<RAM_Interface>
{

    public Emulator Emulator;
    public RAM64K Ram;

    public const int LocationID = 0;

    public const int RunLocation = 0x8000;
    
    

    public List<byte> RamMirror;
    
    [SerializeField]
    public int Run_Location_ID
    {
        get { return Ram[RunLocation + LocationID]; }
        
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Ram = Emulator._ram;

    }

    // Update is called once per frame
    void Update()
    {
        RamMirror = new List<byte>();

        for (int i = 0; i < 1000; i++)
        {
            
            RamMirror.Add(Ram[(ushort)(RunLocation+i)]);
        }
    }
}
