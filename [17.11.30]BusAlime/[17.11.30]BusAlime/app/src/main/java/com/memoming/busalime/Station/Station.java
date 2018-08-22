package com.memoming.busalime.Station;

import java.io.Serializable;

public class Station implements Serializable {

    private String  stNm , arsId , nxtStn , gpsX , gpsY , dist;
    private String  direction, trans;

    public Station () {
        dist        = null;
        direction   = null;
        nxtStn      = null;
    }


    public String   getStNm()               { return stNm; }
    public void     setStNm(String stNm)    { this.stNm = stNm; }

    public String   getArsId()              { return arsId; }
    public void     setArsId(String arsId)  { this.arsId = arsId; }

    public String   getNxtStn()             { return nxtStn; }
    public void     setNxtStn(String nxtStn){ this.nxtStn = nxtStn; }

    public String   getGpsX()               { return gpsX; }
    public void     setGpsX(String posX)    { this.gpsX = posX; }

    public String   getGpsY()               { return gpsY; }
    public void     setGpsY(String posY)    { this.gpsY = posY; }

    public String   getDist()               { return dist; }
    public void     setDist(String dist)    { this.dist = dist; }

    public String getDirection()                { return direction; }
    public void setDirection(String direction)  { this.direction = direction; }

    public String getTrans()                { return trans; }
    public void setTrans(String trans)      { this.trans = trans; }
}
