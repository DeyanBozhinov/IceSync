import * as React from 'react';
import Paper from '@mui/material/Paper';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TablePagination from '@mui/material/TablePagination';
import TableRow from '@mui/material/TableRow';
import CircularProgress from '@mui/material/CircularProgress'
import {
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
} from "@mui/material";
import Button from '@mui/material/Button';
import Backdrop from '@mui/material/Backdrop';
import CheckIcon from "@mui/icons-material/Check";
import CloseIcon from "@mui/icons-material/Close";


const columns = [
  { id: 'workflow_id', label: 'Workflow Id', minWidth: 100 },
  { id: 'name', label: 'Workflow Name', minWidth: 170 },
  { id: 'is_active', label: 'Is Active', minWidth: 100 },
  {
    id: 'multi_exec_behaviour',
    label: 'Multi Exec Behavior',
    minWidth: 100,
  },
  {
    id: 'action',
    label: 'Action',
    minWidth: 100,
  },
];

export default function StickyHeadTable() {
  const [page, setPage] = React.useState(0);
  const [rowsPerPage, setRowsPerPage] = React.useState(10);
  const [workflows, setWorkflows] = React.useState([]);
  const [openDialog, setOpenDialog] = React.useState(false);
  const [openSuccessDialog, setOpenSuccessDialog] = React.useState(false);
  const [tableLoader, setTableLoader] = React.useState(false);
  const [mainLoader, setMainLoader] = React.useState(false);
  const [selectedRow, setSelectedRow] = React.useState(null);
  const BASE_URL = "http://localhost:5054"

  const handleOpenDialog = (row) => {
    setSelectedRow(row);
    setOpenDialog(true);
  };

  // Handle closing the dialog
  const handleCloseDialog = () => {
    setOpenDialog(false);
    setSelectedRow(null);
  };

  const handleCloseSuccessDialog = () => {
    setOpenSuccessDialog(false);
  };

  const handleChangePage = (event, newPage) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(+event.target.value);
    setPage(0);
  };

  const runWorkflow = async(workflowId) => {
    setMainLoader(true);
    setSelectedRow(null);
    setOpenDialog(false);
    const response = await fetch(`${BASE_URL}/workflow/${workflowId}/run`, {
      method: "post",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ id: workflowId })
    });

    if (response.ok) {
      setMainLoader(false);
      setOpenSuccessDialog(true);
    } else {
      const data = await response.json();
      throw new Error("Backend error: " + data.message);
    }
  }

  const getWorkflows = async() => {
    setTableLoader(true);
    const response = await fetch(`${BASE_URL}/workflow/getall`, {
      method: "get",
      headers: {
        "Content-Type": "application/json",
      },
    });
    const data = await response.json();

    if (response.ok) {
      setWorkflows(data);
      setTableLoader(false);
    } else {
      throw new Error("Backend error: " + data.message);
    }
  }

  React.useEffect(() => {
    getWorkflows();
  }, []);

  return (
    <Paper sx={{ width: '100%', overflow: 'hidden',  boxShadow: 9, }}>
      {mainLoader && (<Backdrop />)}
      <TableContainer sx={{ maxHeight: 440 }}>
        <Table stickyHeader aria-label="sticky table">
          <TableHead>
            <TableRow>
              {columns.map((column) => (
                <TableCell
                  id={`${column.id}`}
                  key={`column-${column.id}`}
                  align={column.align}
                  style={{ minWidth: column.minWidth }}
                >
                  {column.label}
                </TableCell>
              ))}
            </TableRow>
          </TableHead>
          <TableBody>
          {tableLoader ? (
            <TableRow>
              <TableCell colSpan={4} align="center">
                <CircularProgress />
              </TableCell>
            </TableRow>
            ) : (workflows
                .slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage)
                .map((row) => {
                  return (
                    <TableRow hover role="checkbox" tabIndex={-1} key={`key-row-${row.workflow_id}`}>
                      {columns.map((column) => {
                        const value = row[column.id];

                        if (column.id === 'action') {
                          return (
                            <TableCell
                             key={`row-${column.id}`}
                            >
                                <Button variant="contained" color="error" size="small" onClick={() => handleOpenDialog(row.workflow_id)}>
                                  Run
                                </Button>
                            </TableCell>
                          );
                        }
                       
                        if (column.id === 'is_active') {
                          return(
                            <TableCell
                              key={`row-${column.id}`}
                            >
                              {row.isActive ? <CheckIcon color="success" /> : <CloseIcon color="error" />}
                            </TableCell>
                          );
                        }

                        return (
                          <TableCell key={`row-${column.id}`} align={column.align}>
                            {column.format && typeof value === 'number'
                              ? column.format(value)
                              : value}
                          </TableCell>
                        );
                      })}
                    </TableRow>
                  );
            }))}
          </TableBody>
        </Table>
      </TableContainer>
      <Dialog open={openDialog} onClose={handleCloseDialog}>
        <DialogTitle>Confirm</DialogTitle>
        <DialogContent>
          <p>Are you sure you want to run the selected workflow?</p>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog} color="primary">
            Cancel
          </Button>
          <Button onClick={() => runWorkflow(selectedRow)} color="error">
            Run
          </Button>
        </DialogActions>
      </Dialog>
      <Dialog
       open={openSuccessDialog}
       onClose={handleCloseSuccessDialog}
      >
        <DialogTitle>Success</DialogTitle>
        <DialogContent>
          <p>The workflow has been executed successfully!</p>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseSuccessDialog} color="primary">
            Close
          </Button>
        </DialogActions>
      </Dialog>
      {!tableLoader && (<TablePagination
        rowsPerPageOptions={[10, 25, 100]}
        component="div"
        count={workflows.length}
        rowsPerPage={rowsPerPage}
        page={page}
        onPageChange={handleChangePage}
        onRowsPerPageChange={handleChangeRowsPerPage}
      />)}
    </Paper>
  );
}