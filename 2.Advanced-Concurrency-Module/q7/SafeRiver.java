/* The SafeRiver composition will be the main program */
public class SafeRiver {
    public static void main(String[] args) {
        System.out.println("Start ...");

        RiverCanvas display = new RiverCanvas();
        River river = new River(display); //River monitor requires "RiverCanvas" object to print some messages

        //Creating two threads for the clans
        Thread hatfieldsThread = new Thread(new Clan(river, "Hatfields"));
        Thread mcCoysThread = new Thread(new Clan(river, "McCoys"));

        //Start the threads
        hatfieldsThread.start();
        mcCoysThread.start();
    }
}

/* CLAN process*/
class Clan implements Runnable {
    private River control;
    private String name;

    Clan(River river, String name) {
        this.name = name;
        control = river;
    }

    public void run() {
        try {
            while (true) {
                control.raiseFlag(name);//Raising the flag and change the turn
                control.checkFlag(name);//Checking flag and turn states
                if (name.equals("Hatfields")) {
                    control.hatfieldsVisit();
                    //Simulating the time required for the 'water' action to be completed
                    Thread.sleep(5000); 
                    control.hatfieldsLeave();
                } else if (name.equals("McCoys")) {
                    control.mcCoysVisit();
                    Thread.sleep(5000);
                    control.mcCoysLeave();
                }
                control.lowerFlag(name);//Lowering the flag

                Thread.sleep(10000);//Simulating some delays between calling threads
            }
        } catch (InterruptedException e) {
        }
    }
}

/*River monitor*/
class River {
    private RiverCanvas display;
    private boolean hatFlag = false; //the state of Hatfields flag
    private boolean mcFlag = false;  //the state of McCoys flag
    private int turn = 1; //indicating turn; initially Hatfields have the turn

    River(RiverCanvas d) {
        display = d;
    }

    //raiseFlag action, from the CLAN process in FSP model
    //both flag and turn states are being changed here (for more simplicity)
    synchronized void raiseFlag(String clan) {
        display.printMessage(String.format("Current Turn = %s", turn == 1 ? "Hatfields" : "McCoys"));

        if (clan.equals("Hatfields")) {
            hatFlag = true;
            turn = 2;
            display.printMessage("Hatfields rasied the flag.");
        } else if (clan.equals("McCoys")) {
            mcFlag = true;
            turn = 1;
            display.printMessage("McCoys rasied the flag.");
        }
    }

    /*
     * In the current implementation, both threads (hatfields and mcCoys) are checking conditions and potentially waiting.
     * Since the logic is related to the synchronization between the two clans, keeping it in a single method is reasonable.
     * 
     * "!(!mcFlag || turn == 1)" is the negation of "when(!mcFlag || turn==1)" from the FSP model.
     * 
     * "!(!hatFlag || turn == 2)" is the negation of "when(!hatFlag || turn==2)" from the FSP model.
    */
    synchronized void checkFlag(String clan) throws InterruptedException {
        while (
            (clan.equals("Hatfields") && !(!mcFlag || turn == 1)) ||
            (clan.equals("McCoys") && !(!hatFlag || turn == 2))
                ) {
            display.printMessage(String.format("%s are waiting ...", clan));
            wait();
        }
    }

    synchronized void hatfieldsVisit() {
        display.printMessage("Hatfields are at the river.");
    }

    synchronized void hatfieldsLeave() {
        display.printMessage("Hatfields have left the river.");
    }

    synchronized void mcCoysVisit() {
        display.printMessage("McCoys are at the river.");
    }

    synchronized void mcCoysLeave() {
        display.printMessage("McCoys have left the river.");
    }

    synchronized void lowerFlag(String clan) {
        if (clan.equals("Hatfields")) {
            hatFlag = false;
            display.printMessage("Hatfields lowered the flag.");
            display.printMessage("-------------------------");
        } else if (clan.equals("McCoys")) {
            mcFlag = false;
            display.printMessage("McCoys lowered the flag.");
            display.printMessage("-------------------------");
        }
        notifyAll();
    }
}

/*The use of RiverCanvas for printing messages can be easily omitted.*/
class RiverCanvas {
    void printMessage(String message) {
        System.out.println(message);
    }
}
