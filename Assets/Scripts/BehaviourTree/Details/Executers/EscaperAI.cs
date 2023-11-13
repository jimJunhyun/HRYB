using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscaperAI : MonoBehaviour
{
	Actor self;
	Actor player;

	Selecter head;

	bool stopped = false;
	// Start is called before the first frame update
	void Start()
    {
        self = GetComponent<Actor>();
		player = GameManager.instance.pActor;

		IsInRange inRange = new IsInRange(self, player.transform, self.sight.GetSightRange, (player.move as PlayerMove).GetSneakDist, ()=>
		{
			(self.move as EscaperMove).SetTarget(player.transform);
		});
		Mover escape = new Mover(self);
		Sequencer escaper = new Sequencer();
		escaper.connecteds.Add(inRange);
		escaper.connecteds.Add(escape);
		

		Inverter inv = new Inverter();
		inv.connected = inRange;
		

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
