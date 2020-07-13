#ifndef GHOST_H
#define GHOST_H
/**
 * @brief Clase ghost que representa a la base de los enemigos del juego
 */
class ghost
{
private:
    int ID;
    int ruta[10];
    /**
     * @note Variable que almacena la posición del enemigo
     */
    int position[2];
    /**
     * @note Variable que contiene la velocidad de movimiento del enemigo
     */
    int speedPatrol;
    int speedPersec;
    int visionRange;

public:
    void moveTo(int x, int y);
    /**
     * Constructor de la clase ghost
     */
    ghost() {}
    /**
     * Destructor de la clase ghost
     */
    ~ghost() {}
    friend class grayGhost;
    friend class redGhost;
    friend class blueGhost;
};

class grayGhost : public ghost
{
public:
    grayGhost(int id)
    {
        ID = id;
    }
};

class redGhost : public ghost
{
public:
    redGhost(int id)
    {
        ID = id;
    }
};

class blueGhost : public ghost
{
public:
    blueGhost(int id)
    {
        ID = id;
    }
    void teleportToEye(int x, int y)
    {
        position[0] = x;
        position[1] = y;
    }
};

#endif