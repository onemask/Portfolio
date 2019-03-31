package com.onemask.busalime.Bus;

import android.content.Context;
import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;


import com.soo.busalime.R;

import java.util.ArrayList;

public class BusAdaptor extends BaseAdapter {

    private Context             mContext;
    private ArrayList<BusItem>  busList;
    private BusItem             busItem;

    public BusAdaptor (Context context , ArrayList<BusItem> busList) {
        this.mContext = context;
        this.busList  = busList;
    }

    @Override
    public int getCount() { return busList.size(); }

    @Override
    public Object getItem(int position) { return busList.get(position); }

    @Override
    public long getItemId(int position) { return position; }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        View view = convertView;
        if ( view == null ) {
            convertView = LayoutInflater.from(mContext).inflate(R.layout.listview_item_busitem, parent, false);
            view = convertView;
        }
        busItem = busList.get(position);
        TextView busItemNm_Tv   = (TextView) view.findViewById(R.id.busItem_busNm);
        TextView busType_Tv     = (TextView) view.findViewById(R.id.busItem_busType);
        TextView busRoutine_Tv  = (TextView) view.findViewById(R.id.busItem_routine);

        busItemNm_Tv.setText(busItem.getBusNm());
        // 3:간선(파란색), 4:지선(녹색), 5:순환(개나리색), 6:광역(빨간색)
        if (busItem.getBusType().equals("3")) {
            busItemNm_Tv.setTextColor(Color.rgb(37, 36, 255));
            busType_Tv.setText("간선버스");
        }
        else if (busItem.getBusType().equals("4")) {
            busItemNm_Tv.setTextColor(Color.rgb(47, 157, 39));
            busType_Tv.setText("지선버스");
        }
        else if (busItem.getBusType().equals("5")) {
            busItemNm_Tv.setTextColor(Color.rgb(255, 187, 0));
            busType_Tv.setText("순환버스");
        }
        else {
            busItemNm_Tv.setTextColor(Color.rgb(255, 54, 54));
            busType_Tv.setText("광역버스");
        }
        busRoutine_Tv.setText(busItem.getStStationNm() + "  ↔ " + busItem.getEnStationNm());

        return view;
    }

}
