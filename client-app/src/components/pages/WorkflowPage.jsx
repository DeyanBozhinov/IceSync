import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Table from "../UI/Table";

const WorkflowPage = () => {
  return (
    <Box
      sx={{ mt: "50px", pl: 20, pr: 20 }}
    >
      <Box>
        <Typography
          variant="h4"
          gutterBottom
          sx={{ fontWeight: "bold" }}
        >
          Workflows
        </Typography>
      </Box>
      <Box>
        <Table />
      </Box>
    </Box>
  );
};

export default WorkflowPage;