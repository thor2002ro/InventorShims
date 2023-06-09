﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;

namespace InventorShims.TranslatorShim
{
    /// <summary>Exports a document to a STL file</summary>
    public class StlExporter
    {
        ///<summary>The document to be exported</summary>
        public Inventor.Document Document { get; set; } = null;

        ///<summary>Defaults to mm.</summary>
        public ImportUnitsTypeEnum Units { get; set; } = ImportUnitsTypeEnum.kMillimeterUnitsType;

        /// <summary>
        /// Setting this to anything other than Custom will cause the exporter to ignore the following variables:<code/>
        /// <see cref="SurfaceDeviation"/>,<code/>
        /// <see cref="NormalDeviation"/>,<code/>
        /// <see cref="MaxEdgeLength"/>,<code/>
        /// <see cref="MaxAspectRatio"/>
        /// </summary>
        public StlResolutionEnum Resolution { get; set; } = StlResolutionEnum.Custom;

        /// <summary>
        /// 
        /// </summary>
        public bool AllowMoveMeshNodes { get; set; } = false;


        ///<summary>backing variable for <see cref="SurfaceDeviation"/></summary>
        private double _surfaceDeviation = 0.005;

        /// <summary>Max distance between facet edges and surface edges (as a percentage of each body's RangeBox size).<code/>
        /// Modifying this value automatically changes <see cref="Resolution"/> to <see cref="StlResolutionEnum.Custom"/><code/>
        /// Valid values: 0 to 100
        /// </summary>
        public double SurfaceDeviation
        {
            get { return _surfaceDeviation; }

            set
            {
                _surfaceDeviation = value;

                Resolution = StlResolutionEnum.Custom;
            }
        }

        ///<summary>backing variable for <see cref="NormalDeviation"/></summary>
        private double _normalDeviation = 10.0;

        /// <summary>The max angle between adjacent face normals of approximated curves.<code/>
        /// Modifying this value automatically changes <see cref="Resolution"/> to <see cref="StlResolutionEnum.Custom"/><code/>
        /// Valid values: 0 to 41
        /// </summary>
        public double NormalDeviation
        {
            get { return _normalDeviation; }

            set
            {
                _normalDeviation = value;

                Resolution = StlResolutionEnum.Custom;
            }
        }

        ///<summary>backing variable for <see cref="MaxEdgeLength"/></summary>
        private double _maxEdgeLength = 0.0;

        /// <summary>Max distance between the grid lines that are placed on a face during the tessellation process (as a percentage of each body's RangeBox size).<code/>
        /// Modifying this value automatically changes <see cref="Resolution"/> to <see cref="StlResolutionEnum.Custom"/><code/>
        /// Valid Values: 0 to 100 (0 to disable)
        /// </summary>
        public double MaxEdgeLength
        {
            get { return _maxEdgeLength; }

            set
            {
                _maxEdgeLength = value;

                Resolution = StlResolutionEnum.Custom;
            }
        }

        ///<summary>backing variable for <see cref="MaxAspectRatio"/></summary>
        private double _maxAspectRatio = 0.0;

        /// <summary>Ratio between height and width of facets.<code/>
        /// Modifying this value automatically changes <see cref="Resolution"/> to <see cref="StlResolutionEnum.Custom"/><code/>
        /// Valid Values: 0 to 21.5 (0 to disable)
        /// </summary>
        public double MaxAspectRatio
        {
            get { return _maxAspectRatio; }

            set
            {
                _maxAspectRatio = value;

                Resolution = StlResolutionEnum.Custom;
            }
        }

        /// <summary>Determines whether an assembly is exported as one or multiple files.</summary>
        public bool OneFilePerPartInstance { get; set; } = false;

        /// <summary>Requires <see cref="Binary"/> = true</summary>
        public bool ExportColors { get; set; } = true;

        /// <summary>
        /// Exported STL file will be binary instead of plaintext ASCII.<code/>
        /// Binary files have a smaller filesize, and support colors.
        /// </summary>
        public bool Binary { get; set; } = true;

        ///<summary>Initializes a new instance of <see cref="StlExporter"/></summary>
        public StlExporter(Inventor.Document Document)
        {
            this.Document = Document;
        }

        ///<summary>Export to STL file with the same folder and filename as the document.</summary>
        public void Export()
        {
            Export(System.IO.Path.ChangeExtension(this.Document.FullFileName, "stl"));
        }

        ///<summary>Export to STL file with the specified full file path.</summary>
        public void Export(string OutputFile)
        {
            TranslatorData oTranslatorData = new TranslatorData(addinGUID: "{533E9A98-FC3B-11D4-8E7E-0010B541CD80}", fullFileName: OutputFile, doc: this.Document);

            NameValueMap op = oTranslatorData.oOptions;

            op.Value["ExportUnits"] = (int)Units - 110080; //Convert ImportUnitsTypeEnum to the integer values expected by the STL exporter
            op.Value["Resolution"] = Resolution;
            op.Value["AllowMoveMeshNode"] = AllowMoveMeshNodes;
            op.Value["SurfaceDeviation"] = SurfaceDeviation;
            op.Value["NormalDeviation"] = NormalDeviation;
            op.Value["MaxEdgeLength"] = MaxEdgeLength;
            op.Value["AspectRatio"] = MaxAspectRatio;
            op.Value["ExportFileStructure"] = Convert.ToInt32(OneFilePerPartInstance);
            op.Value["OutputFileType"] = Convert.ToInt32(!Binary);
            op.Value["ExportColor"] = ExportColors;

            TranslatorAddIn oTranslatorAddIn = (TranslatorAddIn)oTranslatorData.oAppAddIn;

            oTranslatorAddIn.SaveCopyAs(this.Document, oTranslatorData.oContext, oTranslatorData.oOptions, oTranslatorData.oDataMedium);
        }
    }
}
