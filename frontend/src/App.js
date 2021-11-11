import React, { useState, useEffect } from 'react';
import { AgGridColumn, AgGridReact } from 'ag-grid-react';
import { GridOptions } from 'ag-grid-community';


import 'ag-grid-community/dist/styles/ag-grid.css';
import 'ag-grid-community/dist/styles/ag-theme-balham.css';


function App() {
    const storageKey = "karaoke-storage";
    const [rowData, setRowData] = useState(getLocalStorageSongs());
    var gridOptions = {
        defaultColDef: {
          resizable: true,
        },
        columnDefs: [
            { field: 'name', sortable: true, filter: true },
            { field: 'artist', sortable: true, filter: true },
          ],
        rowData: rowData,
        onColumnResized: function (params) {
          console.log(params);
        },
      };
    useEffect(() => {
        var songs = getLocalStorageSongs();
        if(songs.Length == 0)
        {
            fetchSongsRemotely();
            return;
        }
        setRowData(songs);
    }, []);
    function getLocalStorageSongs()
    {
        var songs = window.localStorage.getItem(storageKey);
        if(songs == null)
        {
            return [];
        }
        return JSON.parse(songs);
    }
    function fetchSongsRemotely() {
        fetch('https://localhost:7232/songs')
            .then(result => {
                var test = result.json();
                return test;
            })
            .then(rows =>
                {
                    window.localStorage.setItem(storageKey, JSON.stringify(rows));
                    setRowData(rows);
                });
    }

    return (
        <div className="container ag-theme-balham" style={{ height: 1000, width: 500 }}>
            <AgGridReact gridOptions={gridOptions} />
        </div>
    );
};


export default App;