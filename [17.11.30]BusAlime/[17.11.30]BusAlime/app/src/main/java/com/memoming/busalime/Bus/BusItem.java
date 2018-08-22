package com.memoming.busalime.Bus;

import java.io.Serializable;

public class BusItem implements Serializable {

    // TODO : 해당 bus가 경유하는 경유 목록이 들어가 있는 변수설정
    private String busNm;
    private String routeId;
    private String busType;
    private String stStationNm;
    private String enStationNm;

    public String   getBusNm ()                 { return busNm; }
    public void     setBusNm (String busNm)     { this.busNm = busNm; }

    public String   getRouteId ()               { return routeId; }
    public void     setRouteId (String routeId) { this.routeId = routeId; }

    public String   getBusType ()               { return busType; }
    public void     setBusType (String busType) { this.busType = busType; }

    public String getStStationNm() { return stStationNm; }
    public void setStStationNm(String stStationNm) { this.stStationNm = stStationNm; }

    public String getEnStationNm() { return enStationNm; }
    public void setEnStationNm(String enStationNm) { this.enStationNm = enStationNm; }
}
