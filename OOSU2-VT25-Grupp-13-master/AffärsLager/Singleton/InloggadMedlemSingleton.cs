using EntitetsLager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InloggadMedlemSingleton
{
    private static InloggadMedlemSingleton _instance;
    private Medlem _inloggadMedlem;

    private InloggadMedlemSingleton() { }

    public static InloggadMedlemSingleton GetInstance()
    {
        if (_instance == null)
        {
            _instance = new InloggadMedlemSingleton();
        }
        return _instance;
    }

    public void SetInloggadMedlem(Medlem medlem)
    {
        _inloggadMedlem = medlem;
    }

    public Medlem GetInloggadMedlem()
    {
        return _inloggadMedlem;
    }

    public void Logout()
    {
        _inloggadMedlem = null;
    }
}
