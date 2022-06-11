export class CulsterModel {
  clusterID: number;
  organisationID: number;
  clusterName: string;
  clusterHead: string;
  clusterEmail: string;
  clusterRegion: string;
  clusterlist: {
    id: number;
    text: string;
  }
  organisationlist: {
    id: number;
    text: string;
  }
}
