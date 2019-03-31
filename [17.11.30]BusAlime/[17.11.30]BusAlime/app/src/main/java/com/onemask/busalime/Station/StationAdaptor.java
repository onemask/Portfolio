package com.onemask.busalime.Station;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;

import com.soo.busalime.R;

import java.util.ArrayList;

public class StationAdaptor extends BaseAdapter {

    public StationAdaptor (Context context , ArrayList<Station> stations) {
        this.mContext   = context;
        this.mStations  = stations;
    }

    private Context             mContext;
    private ArrayList<Station>  mStations;

    @Override
    public int getCount() { return mStations.size(); }

    @Override
    public Object getItem(int position) { return mStations.get(position); }

    @Override
    public long getItemId(int position) { return position; }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        View view = convertView;
        if ( view == null ) {
            convertView = LayoutInflater.from(mContext).inflate(R.layout.listview_item_station, parent, false);
            view = convertView;
        }

        final Station station = mStations.get(position);
        TextView stNm   = (TextView) view.findViewById(R.id.station_stNm);
        TextView arsId  = (TextView) view.findViewById(R.id.station_arsId);
        TextView nextSt = (TextView) view.findViewById(R.id.station_nextSt);
        stNm.setText(station.getStNm());
        arsId.setText(station.getArsId());
        if (station.getNxtStn() == null)
            nextSt.setText("("+station.getDirection()+"방면)");
        else
            nextSt.setText("("+station.getNxtStn()+"방면)");

        return view;
    }
}
